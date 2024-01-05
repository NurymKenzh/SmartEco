var CalculationToEnterprise = class {
    constructor(calculationId, enterpriseId) {
        this.CalculationId = calculationId;
        this.EnterpriseId = enterpriseId;
    }
};

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