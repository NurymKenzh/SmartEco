using SmartEco.Models.ASM.Requests.Uprza;
using SmartEco.Models.ASM.Uprza;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static SmartEco.Models.ASM.Uprza.CalculationSetting;

namespace SmartEco.Helpers.ASM
{
    public static class UprzaHelper
    {
        public static bool IsSettingValid(CalculationSetting calcSetting, out string message)
        {
            message = null;
            if (!IsWindSpeedValid(calcSetting.WindSpeedSetting))
            {
                message = "Проверьте настройки скорости ветра";
                return false;
            }
            if (!IsWindDirectionValid(calcSetting.WindDirectionSetting))
            {
                message = "Проверьте настройки направления ветра";
                return false;
            }
            if (calcSetting.IsUsePollutantsList)
            {
                if(!IsPollutantsValid(calcSetting.IsUsePollutantsList, calcSetting.AirPollutantIds))
                {
                    message = "Выберите вещества для расчёта";
                    return false;
                }
            }
            return true;
        }

        public static UprzaRequest MapToRequest(CalculationDetailViewModel calcViewModel)
        {
            var settings = calcViewModel.CalcSettingsViewModel.CalculationSetting;
            var points = calcViewModel.CalcSettingsViewModel.CalculationPoints;
            var rectangles = calcViewModel.CalcSettingsViewModel.CalculationRectangles;

            var airPollutionSources = calcViewModel.CalcToSrcsViewModel.Items
                .Where(s => s.IsInvolved)
                .ToList();
            var pollutants = airPollutionSources
                .SelectMany(s => s.OperationModes
                    .SelectMany(m => m.Emissions
                        .Select(e => e.Pollutant)))
                .GroupBy(p => p.Code)
                .Select(p => p.First())
                .ToList();
            if (settings.IsUsePollutantsList)
            {
                pollutants = pollutants
                    .Where(p => settings.AirPollutantIds.Contains(p.Id))
                    .ToList();
            }

            var uprzaRequest = new UprzaRequest();
            uprzaRequest.AirPollutionSources = airPollutionSources
                .Select(s =>
                {
                    var coordinate3857 = new double[] {0, 0};
                    var coordinateArray = s.SourceInfo.Coordinate3857?.Split(',');
                    if(coordinateArray != null && coordinateArray.Length == 2)
                    {
                        var x3857 = Convert.ToDouble(coordinateArray[0], CultureInfo.InvariantCulture);
                        var y3857 = Convert.ToDouble(coordinateArray[1], CultureInfo.InvariantCulture);
                        coordinate3857 = new double[] { x3857, y3857 };
                    }

                    return new AirPollutionSource
                    {
                        Id = s.Id,
                        Number = s.Number,
                        Name = s.Name,
                        IsOrganized = s.Type.IsOrganized,
                        Methodical = 1,
                        BackgroundRelation = s.SourceInfo.RelationBackgroundId ?? 3,
                        Configuration = new Configuration
                        {
                            Type = s.Type.Id,
                            ReliefCoefficient = s.SourceInfo.TerrainCoefficient ?? 1,
                            IsCovered = s.SourceInfo.IsCovered,
                            IsGas = s.SourceInfo.IsCalculateByGas,
                            IsVerticalDeviation = s.SourceInfo.IsVerticalDeviation,
                            IsTorch = s.SourceInfo.IsSignFlare,
                            Height = decimal.ToDouble(s.SourceInfo.Hight ?? 1),
                            Diameter = decimal.ToDouble(s.SourceInfo.Diameter ?? 1),
                            VerticalDeviation = Convert.ToDouble(s.SourceInfo.AngleDeflection, CultureInfo.InvariantCulture),
                            RotationAngle = Convert.ToDouble(s.SourceInfo.AngleRotation, CultureInfo.InvariantCulture),
                            Point1 = new Point1
                            {
                                //X и Y поменяны местами, т.к. расчёт приходит с неправильными координатами
                                X = coordinate3857[1],
                                Y = coordinate3857[0],
                                Z = 0
                            },

                            FlowTemperature = s.OperationModes.FirstOrDefault()?.GasAirMixture?.Temperature ?? 0,
                            FlowSpeed = s.OperationModes.FirstOrDefault()?.GasAirMixture?.Speed ?? 0,
                            ThermalPower = s.OperationModes.FirstOrDefault()?.GasAirMixture?.ThermalPower,
                            RadiationPower = s.OperationModes.FirstOrDefault()?.GasAirMixture?.PartRadiation,
                            EmissionDensity = s.OperationModes.FirstOrDefault()?.GasAirMixture?.Density,
                        },
                        Humidity = s.OperationModes.FirstOrDefault()?.GasAirMixture?.Humidity,
                        Pressure = s.OperationModes.FirstOrDefault()?.GasAirMixture?.Pressure,

                        Emissions = s.OperationModes
                            .SelectMany(m => m.Emissions)
                            .Where(e => pollutants.Select(p => p.Code).Contains(e.Pollutant.Code))
                            .Select(e => new Emission
                            {
                                PollutantCode = e.Pollutant.Code,
                                Power = decimal.ToDouble(e.MaxGramSec),
                                Coefficient = e.SettlingCoef
                            })
                            .ToList()
                    };
                 })
                .ToList();

            uprzaRequest.ThresholdPdk = settings.ThresholdPdk;
            uprzaRequest.Locality = new Locality
            {
                Square = 682, //temporary
                ReliefCoefficient = 1, //temporary
                StratificationCoefficient = 200 //temporary
            };
            uprzaRequest.Meteo = new Meteo()
            {
                Temperature = 10, //temporary
                WindSpeedSettings = MapToWindSpeedSettings(settings.WindSpeedSetting),
                WindDirectionSettings = MapToWindDirectionSettings(settings.WindDirectionSetting),
                USpeed = 1.5 //temporary
            };
            uprzaRequest.Background = new Background
            {
                Mode = settings.IsUseBackground ? 1 : 0 //temporary
            };
            uprzaRequest.Method = 1; //temporary
            uprzaRequest.ContributorCount = settings.ContributorCount;
            uprzaRequest.UseSummationGroups = settings.IsUseSummationGroups;
            uprzaRequest.CalculatedArea = new CalculatedArea
            {

            };
            uprzaRequest.CalculatedArea = new CalculatedArea
            {
                Rectangles = rectangles
                    .Select(r => new Rectangle
                    {
                        Id = r.Number,
                        CenterPoint = new CenterPoint
                        {
                            //X и Y поменяны местами, т.к. расчёт приходит с неправильными координатами
                            X = r.Ordinate3857,
                            Y = r.Abscissa3857,
                            Z = 0
                        },
                        Width = r.Width, 
                        Height = r.Height, 
                        Length = r.Length,
                        StepByWidth = r.StepByWidth,
                        StepByLength = r.StepByLength
                    }).ToList(),
                Points = points
                    .Select(p => new Point
                    {
                        //X и Y поменяны местами, т.к. расчёт приходит с неправильными координатами
                        Id = p.Number,
                        X = p.Ordinate3857,
                        Y = p.Abscissa3857,
                        Z = 0
                    }).ToList(),

                LivingAreaBorderStep = settings.LivingAreaBorderStep,
                UnitBorderStep = settings.UnitBorderStep,
                SanitaryAreaBorderStep = settings.SanitaryAreaBorderStep
            };
            uprzaRequest.Pollutants = pollutants
                .Select(p => new Pollutant
                {
                    Code = p.Code,
                    Pdk = Convert.ToDouble(p.MpcMaxSingle, CultureInfo.InvariantCulture),
                    PdkLong = Convert.ToDouble(p.MpcAvgDaily, CultureInfo.InvariantCulture)
                }).ToList();
            uprzaRequest.Season = (int)settings.Season;

            return uprzaRequest;
        }

        private static bool IsWindSpeedValid(CalcWindSpeedSetting windSpeedSetting)
        {
            switch (windSpeedSetting.Mode) {
                case CalcWindModes.Auto: 
                    return true;
                case CalcWindModes.Fixed: 
                    return windSpeedSetting.Speed != null;
                case CalcWindModes.IteratingSetNumbers: 
                    return windSpeedSetting.Speeds != null && windSpeedSetting.Speeds.Count != 0;
                case CalcWindModes.IteratingByStep:
                    return windSpeedSetting.StartSpeed != null && windSpeedSetting.EndSpeed != null && windSpeedSetting.StepSpeed != null;
                default: return false;
            }
        }

        private static bool IsWindDirectionValid(CalcWindDirectionSetting windDirectionSetting)
        {
            switch (windDirectionSetting.Mode)
            {
                case CalcWindModes.Auto:
                    return true;
                case CalcWindModes.Fixed:
                    return windDirectionSetting.Direction != null;
                case CalcWindModes.IteratingSetNumbers:
                    return windDirectionSetting.Directions != null && windDirectionSetting.Directions.Count != 0;
                case CalcWindModes.IteratingByStep:
                    return windDirectionSetting.StartDirection != null && windDirectionSetting.EndDirection != null && windDirectionSetting.StepDirection != null;
                default: return false;
            }
        }

        private static bool IsPollutantsValid(bool isUsePollutantsList, List<int> airPollitantIds)
            => isUsePollutantsList && airPollitantIds != null && airPollitantIds.Count != 0;

        private static WindSpeedSettings MapToWindSpeedSettings(CalcWindSpeedSetting windSetting)
        {
            switch (windSetting.Mode)
            {
                case CalcWindModes.IteratingByStep: return new WindSpeedSettings
                {
                    Mode = (int)windSetting.Mode,
                    StartSpeed = windSetting.StartSpeed,
                    EndSpeed = windSetting.EndSpeed,
                    StepSpeed = windSetting.StepSpeed
                };
                case CalcWindModes.IteratingSetNumbers: return new WindSpeedSettings
                {
                    Mode = (int)windSetting.Mode,
                    Speeds = windSetting.Speeds
                };
                case CalcWindModes.Fixed: return new WindSpeedSettings
                {
                    Mode = (int)windSetting.Mode,
                    Speed = windSetting.Speed
                };
                default: return new WindSpeedSettings 
                { 
                    Mode = (int)windSetting.Mode 
                };
            }
        }

        private static WindDirectionSettings MapToWindDirectionSettings(CalcWindDirectionSetting windSetting)
        {
            switch (windSetting.Mode)
            {
                case CalcWindModes.IteratingByStep:
                    return new WindDirectionSettings
                    {
                        Mode = (int)windSetting.Mode,
                        StartDirection = windSetting.StartDirection,
                        EndDirection = windSetting.EndDirection,
                        StepDirection = windSetting.StepDirection
                    };
                case CalcWindModes.IteratingSetNumbers:
                    return new WindDirectionSettings
                    {
                        Mode = (int)windSetting.Mode,
                        Directions = windSetting.Directions
                    };
                case CalcWindModes.Fixed:
                    return new WindDirectionSettings
                    {
                        Mode = (int)windSetting.Mode,
                        Direction = windSetting.Direction
                    };
                default:
                    return new WindDirectionSettings
                    {
                        Mode = (int)windSetting.Mode
                    };
            }
        }
    }
}
