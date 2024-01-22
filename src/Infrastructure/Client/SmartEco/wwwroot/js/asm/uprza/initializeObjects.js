//Enterprises
var CalculationToEnterprise = class {
    constructor(calculationId, enterpriseId) {
        this.CalculationId = calculationId;
        this.EnterpriseId = enterpriseId;
    }
};

//Sources
var SourceFilter = class {
    CalculationId;
    EnterpriseIds;

    SortOrder;
    PageNumber;
    PageSize;
};

var CalculationToSource = class {
    constructor(calculationId, sourceId) {
        this.CalculationId = calculationId;
        this.SourceId = sourceId;
    }
};

//Settings
var CalculationPoint = class {
    constructor(calculationId, number) {
        this.CalculationId = calculationId;
        this.Number = number;
    }

    AbscissaX;
    OrdinateY;
    ApplicateZ;
};

var CalculationRectangle = class {
    constructor(calculationId, number) {
        this.CalculationId = calculationId;
        this.Number = number;
    }

    AbscissaX;
    OrdinateY;
    Width;
    Length;
    Height;
    StepByWidth;
    StepByLength;
};

const CalcWindModes = Object.freeze({
    Auto: 0,
    Fixed: 1,
    IteratingSetNumbers: 2,
    IteratingByStep: 3
})

var CalculationSetting = class {
    constructor(calculationId) {
        this.CalculationId = calculationId;
    }

    WindSpeedSetting;
    WindDirectionSetting;

    ContributorCount;
    ThresholdPdk;
    Season;

    IsUseSummationGroups;
    IsUseBackground;
    IsUseBuilding;

    UnitBorderStep;
    SanitaryAreaBorderStep;
    LivingAreaBorderStep;

    IsUsePollutantsList;
    AirPollutantIds;
};

var CalcWindSpeedSetting = class {
    Mode;
    Speed;
    StartSpeed;
    EndSpeed;
    StepSpeed;
    Speeds;
};

var CalcWindDirectionSetting = class {
    Mode;
    Direction;
    StartDirection;
    EndDirection;
    StepDirection;
    Directions;
};

const CalcStatuses = Object.freeze({
    New: 1,
    Configuration: 2,
    Initiated: 3,
    Error: 4,
    Done: 5
})