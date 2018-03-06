--
-- Utility Tables
--

create table MessageQueue (
  Id int identity,
  primary key(Id),
  Type varchar(255),
  Payload varbinary(max),
  PublishedOnUTC datetime
);

-- select * from MessageQueue order by Id desc

--
-- Reference Data
--

create table UnitOfMeasure (
  Id int not null identity,
  primary key(Id),
  Code varchar(100) not null,
  unique(Code),
  Name nvarchar(1000) not null,
  MandatoryQoE int
);

create table Currency (
  Id int not null identity,
  primary key(Id),
  Code varchar(100) not null,
  unique(Code),
  Name nvarchar(1000) not null
);

insert into Currency(Code, Name) values('USD', 'US Dollars');

create table AwardStatus (
  Id int not null identity,
  primary key(Id),
  Code varchar(100) not null,
  unique(Code),
  Name nvarchar(1000) not null
);

insert into AwardStatus(Code, Name) values('sole', 'Sole');
insert into AwardStatus(Code, Name) values('dual', 'Dual');
insert into AwardStatus(Code, Name) values('multi', 'Multi');
insert into AwardStatus(Code, Name) values('optional', 'Optional');

create table OrderFrom (
  Id int not null identity,
  primary key(Id),
  Code varchar(100) not null,
  unique(Code),
  Name nvarchar(1000) not null,
  IsContractedVendorOrderingAllowed bit not null,
  IsDistributorOrderingAllowed bit not null
);

insert into OrderFrom(Code, Name, IsContractedVendorOrderingAllowed, IsDistributorOrderingAllowed) values('direct', 'Direct', 1, 0);
insert into OrderFrom(Code, Name, IsContractedVendorOrderingAllowed, IsDistributorOrderingAllowed) values('distirbuted', 'Distributed', 0, 1);
insert into OrderFrom(Code, Name, IsContractedVendorOrderingAllowed, IsDistributorOrderingAllowed) values('both', 'Both', 1, 1);

create table AmendmentStatus (
  Id int not null identity,
  primary key(Id),
  Code varchar(100) not null,
  unique(Code),
  Name nvarchar(1000) not null,
  IsExecuted bit not null,
);

insert into AmendmentStatus(Code, Name, IsExecuted) values('val', 'Executed', 1);
insert into AmendmentStatus(Code, Name, IsExecuted) values('ini', 'Drafting', 0);
insert into AmendmentStatus(Code, Name, IsExecuted) values('nego', 'Negotiations', 0);
insert into AmendmentStatus(Code, Name, IsExecuted) values('sgn_pend', 'Locked for Approval', 0);
insert into AmendmentStatus(Code, Name, IsExecuted) values('can', 'Canceled', 0);

--
-- Vendor
--

create table Vendor (
  Id int not null identity,
  primary key(Id),
  SNV int not null,
  unique(SNV),
  Name nvarchar(255) not null
);

--
-- Item
--

create table Item (
  Id int not null identity,
  primary key(Id),
  UIN int not null,
  unique(UIN),
  ManufacturerVendorId int,
  foreign key(ManufacturerVendorId) references Vendor(Id),
  ManufacturerCatalogNumber varchar(100),
  Description nvarchar(1000)
);

--
-- Membership
--

create table MembershipHierarchyLevel (
  Id int not null identity,
  primary key(Id),
  OrderBy int not null,
  Name nvarchar(100)
);

insert into MembershipHierarchyLevel(OrderBy, Name) values(1, 'Organization');
insert into MembershipHierarchyLevel(OrderBy, Name) values(2, 'Company');
insert into MembershipHierarchyLevel(OrderBy, Name) values(3, 'Division');
insert into MembershipHierarchyLevel(OrderBy, Name) values(4, 'Group');
insert into MembershipHierarchyLevel(OrderBy, Name) values(5, 'Market');
insert into MembershipHierarchyLevel(OrderBy, Name) values(6, 'Facility');
insert into MembershipHierarchyLevel(OrderBy, Name) values(7, 'Location');

--TODO: think about how we handle this
create table Member (
  Id int not null identity,
  primary key(Id),
  ParentId int,
  foreign key(ParentId) references Member(Id),
  LevelId int not null,
  foreign key(LevelId) references MembershipHierarchyLevel(Id),
  Code varchar(100) not null,
  unique(LevelId, Code),
  Name nvarchar(1000) not null
);

--
-- Categorization
--

create table CategorizationHierarchyLevel (
  Id int not null identity,
  primary key(Id),
  OrderBy int not null,
  Name nvarchar(100)
);

insert into CategorizationHierarchyLevel(OrderBy, Name) values(1, 'Category');
insert into CategorizationHierarchyLevel(OrderBy, Name) values(2, 'SubCategory');

--TODO: think about how we handle this
create table Category (
  Id int not null identity,
  primary key(Id),
  ParentId int,
  foreign key(ParentId) references Category(Id),
  LevelId int not null,
  foreign key(LevelId) references CategorizationHierarchyLevel(Id),
  Code varchar(100) not null,
  unique(LevelId, Code),
  Name nvarchar(1000) not null
);

--
-- Contract
--

create table Contract (
  Id int not null identity,
  primary key(Id),
  ContractNumber int not null,
  unique(ContractNumber),
  EffectiveDate date not null,
  ExpirationDate date
);

create table ContractAmendment (
  Id int not null identity,
  ContractId int not null,
  foreign key(ContractId) references Contract(Id),
  primary key(ContractId, Id),
  CycleNumber int not null,
  AmendmentNumber int not null,
  unique(ContractId, CycleNumber, AmendmentNumber),
  EffectiveDate date not null,
  VisibilityDate date not null,
  StatusId int not null,
  foreign key(StatusId) references AmendmentStatus(Id),
  
  OrderFromId int not null,
  foreign key(OrderFromId) references OrderFrom(Id)
);

create table ContractCategory (
  Id int not null identity,
  ContractId int not null,
  foreign key(ContractId) references Contract(Id),
  primary key(ContractId, Id),
  CategoryId int not null,
  foreign key(CategoryId) references Category(Id),
  unique(ContractId, CategoryId),
  AwardStatusId int not null,
  foreign key(AwardStatusId) references AwardStatus(Id),
);

create table ContractTier (
  Id int not null identity,
  ContractId int not null,
  foreign key(ContractId) references Contract(Id),
  primary key(ContractId, Id),
  TierNumber int not null,
  BeginDate date,
  EndDate date
);

create table ContractVendor (
  Id int not null identity,
  ContractId int not null,
  foreign key(ContractId) references Contract(Id),
  primary key(ContractId, Id),
  VendorId int not null,
  Role varchar(100) not null,
  BeginDate date,
  EndDate date
);

create table ContractVendorItem (
  Id int not null identity,
  ContractId int not null,
  primary key(ContractId, Id),
  ContractVendorId int not null,
  foreign key(ContractId, ContractVendorId) references ContractVendor(ContractId, Id),
  ItemId int not null,
  foreign key(ItemId) references Item(Id),
  BeginDate date,
  EndDate date
);

create table ContractVendorItemPackage (
  Id int not null identity,
  ContractId int not null,
  primary key(ContractId, Id),
  ContractVendorItemId int not null,
  foreign key(ContractId, ContractVendorItemId) references ContractVendorItem(ContractId, Id),
  UnitOfMeasureId int not null,
  foreign key(UnitOfMeasureId) references UnitOfMeasure(Id),
  QuantityOfEach int not null,
  ReorderNumber varchar(100),
  BeginDate date,
  EndDate date
);

create table ContractVendorItemPackagePrice (
  Id int not null identity,
  ContractId int not null,
  primary key(ContractId, Id),
  ContractVendorItemPackageId int not null,
  foreign key(ContractId, ContractVendorItemPackageId) references ContractVendorItemPackage(ContractId, Id),
  ContractTierId int not null,
  foreign key(ContractId, ContractTierId) references ContractTier(ContractId, Id),
  CurrencyId int not null,
  foreign key(CurrencyId) references Currency(Id),
  Price money not null,
  BeginDate date,
  EndDate date
);

create table Markup (
  Id int not null identity,
  primary key(Id),
  MemberId int not null,
  foreign key(MemberId) references Member(Id),
  DistributorId int not null,
  foreign key(DistributorId) references Vendor(Id),
  ContractId int,
  foreign key(ContractId) references Contract(Id),
  Percentage decimal(4, 2)
);
