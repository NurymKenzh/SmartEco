var Filter = class {
    EnterpriseId;
    NameFilter;
    NumberFilter;
    RelationFilter;

    SortOrder;
    PageNumber;
    PageSize;
};

var AirPollutionSource = class {
    Id;
    Number;
    Name;
    IsActive;
    TypeId;
    SourceInfo;

    SourceIndSite;
    SourceWorkshop;
    SourceArea;
};

var AirPollutionSourceInfo = class {
    SourceId;
    Coordinate;
    TerrainCoefficient;
    IsCalculateByGas;
    IsVerticalDeviation;
    AngleDeflection;
    AngleRotation;
    IsCovered;
    IsSignFlare;
    Hight;
    Diameter;
    RelationBackground;
};

var SourceIndSite = class {
    constructor(airPollutionSourceId, indSiteEnterpriseId) {
        this.AirPollutionSourceId = airPollutionSourceId;
        this.IndSiteEnterpriseId = indSiteEnterpriseId;
    }
};

var SourceWorkshop = class {
    constructor(airPollutionSourceId, workshopId) {
        this.AirPollutionSourceId = airPollutionSourceId;
        this.WorkshopId = workshopId;
    }
};

var SourceArea = class {
    constructor(airPollutionSourceId, areaId) {
        this.AirPollutionSourceId = airPollutionSourceId;
        this.AreaId = areaId;
    }
};