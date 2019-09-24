from osgeo import ogr
import os

# Get a Layer
inShapefile = "SHP.shp"
inDriver = ogr.GetDriverByName("ESRI Shapefile")
inDataSource = inDriver.Open(inShapefile, 0)
inLayer = inDataSource.GetLayer()

# Collect all Geometry
geomcol = ogr.Geometry(ogr.wkbGeometryCollection)
for feature in inLayer:
    geomcol.AddGeometry(feature.GetGeometryRef())

# Calculate convex hull
convexhull = geomcol.ConvexHull()

# Save extent to a new Shapefile
outShapefile = "MaskExtent.shp"
outDriver = ogr.GetDriverByName("ESRI Shapefile")

# Remove output shapefile if it already exists
if os.path.exists(outShapefile):
    outDriver.DeleteDataSource(outShapefile)

# Create the output shapefile
outDataSource = outDriver.CreateDataSource(outShapefile)
outLayer = outDataSource.CreateLayer("", geom_type=ogr.wkbPolygon)

# Add an ID field
idField = ogr.FieldDefn("id", ogr.OFTInteger)
outLayer.CreateField(idField)

# Create the feature and set values
featureDefn = outLayer.GetLayerDefn()
feature = ogr.Feature(featureDefn)
feature.SetGeometry(convexhull)
feature.SetField("id", 1)
outLayer.CreateFeature(feature)
feature = None

# Save and close DataSource
inDataSource = None
outDataSource = None
