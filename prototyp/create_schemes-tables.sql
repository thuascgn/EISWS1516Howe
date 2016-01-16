CREATE SCHEMA rule_service;

CREATE table rule_service.tbl_rules(
	id integer not null primary key auto_increment,
    condition_Sender text,
	condition_KeywordAccountingText text,
	condition_KeycharsDocumentNumber text,
	attribution_Department text,
    attribution_ContactPerson text,
    attribution_Project text,
    attribution_Account text,
	attribution_CostCenter text
);

CREATE table rule_service.tbl_documents(
	id integer not null primary key auto_increment,
    documentGuid text not null,
    path text,
    sender text,
    documentNumber text, 	
    accountingText text
);

CREATE SCHEMA task_service;

USE task_service;

CREATE TABLE task_service.tbl_tasks(
	id integer not null primary key unique auto_increment,
    priority integer not null default 0,
    channel text not null,
    isLocked boolean not null default false,
    /*
    Dokumentdaten*/
    document_Id integer,
    document_Guid text not null,
    document_Path text,
    document_Sender text,
    document_Number text,
    document_AccountingText text,
    /*
    Regel*/
    ruleCondition_Sender text,
    ruleCondition_KeywordAccountingText text, 
    ruleCondition_KeycharsDocumentNumber text, 
    ruleAttribution_Department text, 
    ruleAttribution_ProjectNumber integer, 
    ruleAttribution_ContactPerson text,
    ruleAttribution_Account text, 
    ruleAttribution_CostCenter text
);
