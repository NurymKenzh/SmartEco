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
    Type;
    SourceInfo;

    SourceIndSite;
    SourceWorkshop;
    SourceArea;
};

var AirPollutionSourceInfo = class {
    SourceId;
    Coordinate;
    Coordinate3857;
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
    Length;
    Width;
};

var AirPollutionSourceType = class {
    constructor(isOrganized, name) {
        this.Name = name;
        this.IsOrganized = isOrganized;
    }
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

//Operation modes and Emissions

var OperationMode = class {
    Id;
    Name;
    WorkedTime;
    SourceId;

    GasAirMixture
};

var GasAirMixture = class {
    OperationModeId;
    Temperature;
    Pressure;
    Speed;
    Volume;
    Humidity;
    Density;
    ThermalPower;
    PartRadiation;
}

var Emission = class {
    Id;
    PollutantId;
    OperationModeId;
    MaxGramSec;
    MaxMilligramMeter;
    GrossTonYear;
    SettlingCoef;
    EnteredDate;
}