using SmartEco.Models.ASM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartEco.Extensions.ASM
{
    public static class KatoExtension
    {
        #region KATO by match input

        /// <summary>
        /// Get KATO codes from enterprises
        /// </summary>
        /// <param name="katoEnterprises">KATO from database</param>
        /// <example></example>
        /// <returns>Grouping list codes</returns>
        internal static List<string> GetCodes(this List<KatoEnterprise> katoEnterprises)
            => katoEnterprises
                .GroupBy(kato => new { kato.Code })
                .Select(group => group.First().Code)
                .ToList();

        /// <summary>
        /// Get root KATO, that contain match input.
        /// Necessary to get a branch containing all possible KATO
        /// </summary>
        /// <param name="katoHierarchies">KATO catalog, that cointain children elements</param>
        /// <param name="katoCodeName">Match input</param>
        /// <example>Count: 16218 ==> Count: 1 - "230000000" Атырауская область -> Children.Count: 8 - [ "233600000" Жылыойский район, ... ]</example>
        /// <returns>Root branches with children elements</returns>
        internal static List<KatoCatalogDownHierarchy> GetRootKatoMatch(this List<KatoCatalogDownHierarchy> katoHierarchies, string katoCodeName)
        {
            var rootKato = new List<KatoCatalogDownHierarchy>();
            foreach (var root in katoHierarchies.Where(kato => kato.ParentId == 0))
            {
                var result = root
                    .RecursiveDeep(x => x.Children)
                    .Where(x => x.Code.StartsWith(katoCodeName) || x.Name.ToLower().StartsWith(katoCodeName));
                if (result.Any())
                    rootKato.Add(root);
            }
            return rootKato;
        }

        /// <summary>
        /// Get root KATO children, that contain enterprise codes from database.
        /// Recursively goes deep and takes the appropriate child element
        /// </summary>
        /// <param name="katoHierarchies">Filtered KATO catalog (root branches), that cointain children elements</param>
        /// <param name="codes">KATO codes from enterprises</param>
        /// <example>Count: 1 ==> Count: 4 - [ "233620100" г.Кульсары, ... ]</example>
        /// <returns>Match list children from root branches</returns>
        internal static List<KatoCatalogDownHierarchy> GetRootKatoChildren(this List<KatoCatalogDownHierarchy> katoHierarchies, List<string> codes)
        {
            var rootKatoChildren = new List<KatoCatalogDownHierarchy>();
            foreach (var root in katoHierarchies)
            {
                var result = root
                    .RecursiveDeep(x => x.Children)
                    .Where(x => codes.Contains(x.Code));
                if (result != null)
                    rootKatoChildren.AddRange(result);
            }
            return rootKatoChildren;
        }

        /// <summary>
        /// Get KATO list with parents element.
        /// Necessary to get those that match with KATo children codes
        /// </summary>
        /// <param name="katoHierarchy">List to filter</param>
        /// <param name="rootKatoChilrenDownHierarchy">Comparison list</param>
        /// <example>Count: 4 ==> Count: 4 - [ "233620100" г.Кульсары, ... ] -> Parent - "233600000" Жылыойский район -> Parent - "230000000" Атырауская область</example>
        /// <returns>Match list KATO with parent elements</returns>
        internal static List<KatoCatalogUpHierarchy> GetKatoContainChildren(this List<KatoCatalogUpHierarchy> katoHierarchy, List<KatoCatalogDownHierarchy> rootKatoChilrenDownHierarchy)
            => katoHierarchy
                .Where(katoUp => rootKatoChilrenDownHierarchy
                    .Select(katoDown => katoDown.Code)
                    .Contains(katoUp.Code))
                .ToList();

        #endregion KATO by match input

        /// <summary>
        /// Mapping base KATO catalog to hierarchical list.
        /// Each element contains its parent element
        /// </summary>
        /// <param name="katoCatalogs"></param>
        /// <returns>Mapped list</returns>
        internal static List<KatoCatalogUpHierarchy> MapToParentHierarchy(this List<KatoCatalog> katoCatalogs)
        {
            var katoHierarchy = katoCatalogs
                .Select(kato => new KatoCatalogUpHierarchy
                {
                    Id = kato.Id,
                    ParentId = kato.ParentId,
                    Name = kato.Name,
                    Code = kato.Code,
                }).ToList();

            //Set parent
            var lookup = katoHierarchy.ToLookup(c => c.Id);
            katoHierarchy.ForEach(kato => kato.Parent = lookup[kato.ParentId].SingleOrDefault());
            return katoHierarchy;
        }

        /// <summary>
        /// Mapping base KATO catalog to hierarchical list.
        /// Each element contains a list of dependent children
        /// </summary>
        /// <param name="katoCatalogs"></param>
        /// <returns>Mapped list</returns>
        internal static List<KatoCatalogDownHierarchy> MapToChildrenHierarchy(this List<KatoCatalog> katoCatalogs)
        {
            var katoHierarchy = katoCatalogs
                .Select(kato => new KatoCatalogDownHierarchy
                {
                    Id = kato.Id,
                    ParentId = kato.ParentId,
                    Name = kato.Name,
                    Code = kato.Code
                }).ToList();

            //Set childrens
            var lookup = katoHierarchy.ToLookup(c => c.ParentId);
            katoHierarchy.ForEach(kato => kato.Children = lookup[kato.Id].ToList());
            return katoHierarchy;
        }

        /// <summary>
        /// Get parents up the hierarchy
        /// </summary>
        /// <param name="katoHierarchies">List KATO contins parent elemnts</param>
        /// <example>Count: 4 ==> Count: 15</example>
        /// <returns>List all parents of the base list up in the hierarchy</returns>
        internal static IEnumerable<KatoCatalog> GetParents(this List<KatoCatalogUpHierarchy> katoHierarchies)
        {
            foreach (var kato in katoHierarchies)
            {
                yield return kato;

                if (kato.Parent != null)
                {
                    foreach (var child in new List<KatoCatalogUpHierarchy> { kato.Parent }.GetParents())
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Get a list of dependent elements down the hierarchy
        /// </summary>
        /// <typeparam name="T">Generic type of source</typeparam>
        /// <param name="source">Root element</param>
        /// <param name="selector">Elements to search</param>
        /// <example>Count: 1 ==> Count: 20</example>
        /// <returns>All deep items in one list </returns>
        internal static IEnumerable<T> RecursiveDeep<T>(this T source, Func<T, IEnumerable<T>> selector)
        {
            return selector(source)
                .SelectMany(c => RecursiveDeep(c, selector))
                .Concat(new[] { source });
        }
    }
}
