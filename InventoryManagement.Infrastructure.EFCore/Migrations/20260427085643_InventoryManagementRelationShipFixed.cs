using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Infrastructure.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class InventoryManagementRelationShipFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "TaxTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "Storages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "Storages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ManagerPersonId",
                table: "Storages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProvinceId",
                table: "Storages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "StorageProducts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StorageId",
                table: "StorageProducts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "StorageLocations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StorageId",
                table: "StorageLocations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "ProductSerials",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BrandId",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TaxTypeId",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnitDetailsId",
                table: "Products",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PriceTypeId",
                table: "ProductPrices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "ProductPrices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "ProductPrices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "ProductCreateSeries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "ProductBatches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AttributeId",
                table: "ProductAttributeValues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "ProductAttributeValues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "PriceTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "InventoryTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FailureTypeId",
                table: "InventoryTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "InventoryTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductCreateSeriesId",
                table: "InventoryTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "InventoryTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StorageId",
                table: "InventoryTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Categories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "Categories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "Barcodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            
            migrationBuilder.CreateIndex(
                name: "IX_TaxTypes_CompanyId",
                table: "TaxTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_BranchId",
                table: "Storages",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_CityId",
                table: "Storages",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_ManagerPersonId",
                table: "Storages",
                column: "ManagerPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_ProvinceId",
                table: "Storages",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageProducts_ProductId",
                table: "StorageProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageProducts_StorageId",
                table: "StorageProducts",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_ParentId",
                table: "StorageLocations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_StorageId",
                table: "StorageLocations",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSerials_ProductId",
                table: "ProductSerials",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CompanyId",
                table: "Products",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TaxTypeId",
                table: "Products",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitDetailsId",
                table: "Products",
                column: "UnitDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitId",
                table: "Products",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_PriceTypeId",
                table: "ProductPrices",
                column: "PriceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ProductId",
                table: "ProductPrices",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_UnitId",
                table: "ProductPrices",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCreateSeries_ProductId",
                table: "ProductCreateSeries",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_ProductId",
                table: "ProductBatches",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_AttributeId",
                table: "ProductAttributeValues",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductId",
                table: "ProductAttributeValues",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceTypes_CompanyId",
                table: "PriceTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_BranchId",
                table: "InventoryTransactions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_FailureTypeId",
                table: "InventoryTransactions",
                column: "FailureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_LocationId",
                table: "InventoryTransactions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductCreateSeriesId",
                table: "InventoryTransactions",
                column: "ProductCreateSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductId",
                table: "InventoryTransactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_StorageId",
                table: "InventoryTransactions",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CompanyId",
                table: "Categories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_ProductId",
                table: "Barcodes",
                column: "ProductId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_Barcodes_Products_ProductId",
                table: "Barcodes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories",
                column: "ParentId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Companies_CompanyId",
                table: "Categories",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Branches_BranchId",
                table: "InventoryTransactions",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_FailureTypes_FailureTypeId",
                table: "InventoryTransactions",
                column: "FailureTypeId",
                principalTable: "FailureTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_ProductCreateSeries_ProductCreateSeriesId",
                table: "InventoryTransactions",
                column: "ProductCreateSeriesId",
                principalTable: "ProductCreateSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_StorageLocations_LocationId",
                table: "InventoryTransactions",
                column: "LocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Storages_StorageId",
                table: "InventoryTransactions",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceTypes_Companies_CompanyId",
                table: "PriceTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_ProductAttributes_AttributeId",
                table: "ProductAttributeValues",
                column: "AttributeId",
                principalTable: "ProductAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId",
                table: "ProductAttributeValues",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductBatches_Products_ProductId",
                table: "ProductBatches",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCreateSeries_Products_ProductId",
                table: "ProductCreateSeries",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_PriceTypes_PriceTypeId",
                table: "ProductPrices",
                column: "PriceTypeId",
                principalTable: "PriceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_Products_ProductId",
                table: "ProductPrices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_Units_UnitId",
                table: "ProductPrices",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Companies_CompanyId",
                table: "Products",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_TaxTypes_TaxTypeId",
                table: "Products",
                column: "TaxTypeId",
                principalTable: "TaxTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Units_UnitDetailsId",
                table: "Products",
                column: "UnitDetailsId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Units_UnitId",
                table: "Products",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSerials_Products_ProductId",
                table: "ProductSerials",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StorageLocations_StorageLocations_ParentId",
                table: "StorageLocations",
                column: "ParentId",
                principalTable: "StorageLocations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StorageLocations_Storages_StorageId",
                table: "StorageLocations",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StorageProducts_Products_ProductId",
                table: "StorageProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StorageProducts_Storages_StorageId",
                table: "StorageProducts",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Branches_BranchId",
                table: "Storages",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Cities_CityId",
                table: "Storages",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Persons_ManagerPersonId",
                table: "Storages",
                column: "ManagerPersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Provinces_ProvinceId",
                table: "Storages",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaxTypes_Companies_CompanyId",
                table: "TaxTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barcodes_Products_ProductId",
                table: "Barcodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Companies_CompanyId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Branches_BranchId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_FailureTypes_FailureTypeId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_ProductCreateSeries_ProductCreateSeriesId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_StorageLocations_LocationId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Storages_StorageId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceTypes_Companies_CompanyId",
                table: "PriceTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_ProductAttributes_AttributeId",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductBatches_Products_ProductId",
                table: "ProductBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCreateSeries_Products_ProductId",
                table: "ProductCreateSeries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_PriceTypes_PriceTypeId",
                table: "ProductPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_Products_ProductId",
                table: "ProductPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_Units_UnitId",
                table: "ProductPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Companies_CompanyId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_TaxTypes_TaxTypeId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Units_UnitDetailsId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Units_UnitId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSerials_Products_ProductId",
                table: "ProductSerials");

            migrationBuilder.DropForeignKey(
                name: "FK_StorageLocations_StorageLocations_ParentId",
                table: "StorageLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_StorageLocations_Storages_StorageId",
                table: "StorageLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_StorageProducts_Products_ProductId",
                table: "StorageProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_StorageProducts_Storages_StorageId",
                table: "StorageProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Branches_BranchId",
                table: "Storages");

            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Cities_CityId",
                table: "Storages");

            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Persons_ManagerPersonId",
                table: "Storages");

            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Provinces_ProvinceId",
                table: "Storages");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxTypes_Companies_CompanyId",
                table: "TaxTypes");

            

            migrationBuilder.DropIndex(
                name: "IX_TaxTypes_CompanyId",
                table: "TaxTypes");

            migrationBuilder.DropIndex(
                name: "IX_Storages_BranchId",
                table: "Storages");

            migrationBuilder.DropIndex(
                name: "IX_Storages_CityId",
                table: "Storages");

            migrationBuilder.DropIndex(
                name: "IX_Storages_ManagerPersonId",
                table: "Storages");

            migrationBuilder.DropIndex(
                name: "IX_Storages_ProvinceId",
                table: "Storages");

            migrationBuilder.DropIndex(
                name: "IX_StorageProducts_ProductId",
                table: "StorageProducts");

            migrationBuilder.DropIndex(
                name: "IX_StorageProducts_StorageId",
                table: "StorageProducts");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_ParentId",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_StorageLocations_StorageId",
                table: "StorageLocations");

            migrationBuilder.DropIndex(
                name: "IX_ProductSerials_ProductId",
                table: "ProductSerials");

            migrationBuilder.DropIndex(
                name: "IX_Products_BrandId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CompanyId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TaxTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UnitDetailsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UnitId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_PriceTypeId",
                table: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_ProductId",
                table: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_UnitId",
                table: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_ProductCreateSeries_ProductId",
                table: "ProductCreateSeries");

            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_ProductId",
                table: "ProductBatches");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_AttributeId",
                table: "ProductAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_ProductId",
                table: "ProductAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_PriceTypes_CompanyId",
                table: "PriceTypes");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_BranchId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_FailureTypeId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_LocationId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_ProductCreateSeriesId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_ProductId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_StorageId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CompanyId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Barcodes_ProductId",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "TaxTypes");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "ManagerPersonId",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "StorageProducts");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "StorageProducts");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "StorageLocations");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductSerials");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TaxTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitDetailsId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceTypeId",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductCreateSeries");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "AttributeId",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "PriceTypes");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "FailureTypeId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ProductCreateSeriesId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Barcodes");
        }
    }
}
