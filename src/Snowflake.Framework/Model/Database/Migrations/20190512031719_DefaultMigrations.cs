﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Snowflake.Model.Database.Migrations
{
    public partial class DefaultMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationProfiles",
                columns: table => new
                {
                    ValueCollectionGuid = table.Column<Guid>(nullable: false),
                    ConfigurationSource = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationProfiles", x => x.ValueCollectionGuid);
                });

            migrationBuilder.CreateTable(
                name: "ControllerElementMappings",
                columns: table => new
                {
                    ControllerID = table.Column<string>(nullable: false),
                    DeviceID = table.Column<string>(nullable: false),
                    ProfileName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControllerElementMappings", x => new { x.ControllerID, x.DeviceID, x.ProfileName });
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    RecordID = table.Column<Guid>(nullable: false),
                    RecordType = table.Column<string>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    MimeType = table.Column<string>(nullable: true),
                    Platform = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.RecordID);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationValues",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    SectionKey = table.Column<string>(nullable: false),
                    OptionKey = table.Column<string>(nullable: false),
                    ValueCollectionGuid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationValues", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ConfigurationValues_ConfigurationProfiles_ValueCollectionGuid",
                        column: x => x.ValueCollectionGuid,
                        principalTable: "ConfigurationProfiles",
                        principalColumn: "ValueCollectionGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MappedControllerElements",
                columns: table => new
                {
                    LayoutElement = table.Column<string>(nullable: false),
                    ControllerID = table.Column<string>(nullable: false),
                    DeviceID = table.Column<string>(nullable: false),
                    ProfileName = table.Column<string>(nullable: false),
                    DeviceElement = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappedControllerElements", x => new { x.ControllerID, x.DeviceID, x.ProfileName, x.LayoutElement });
                    table.ForeignKey(
                        name: "FK_MappedControllerElements_ControllerElementMappings_ControllerID_DeviceID_ProfileName",
                        columns: x => new { x.ControllerID, x.DeviceID, x.ProfileName },
                        principalTable: "ControllerElementMappings",
                        principalColumns: new[] { "ControllerID", "DeviceID", "ProfileName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameRecordsConfigurationProfiles",
                columns: table => new
                {
                    GameID = table.Column<Guid>(nullable: false),
                    ProfileName = table.Column<string>(nullable: false),
                    ConfigurationSource = table.Column<string>(nullable: false),
                    ProfileID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRecordsConfigurationProfiles", x => new { x.ProfileName, x.GameID, x.ConfigurationSource });
                    table.ForeignKey(
                        name: "FK_GameRecordsConfigurationProfiles_Records_GameID",
                        column: x => x.GameID,
                        principalTable: "Records",
                        principalColumn: "RecordID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameRecordsConfigurationProfiles_ConfigurationProfiles_ProfileID",
                        column: x => x.ProfileID,
                        principalTable: "ConfigurationProfiles",
                        principalColumn: "ValueCollectionGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Metadata",
                columns: table => new
                {
                    RecordMetadataID = table.Column<Guid>(nullable: false),
                    RecordID = table.Column<Guid>(nullable: false),
                    MetadataKey = table.Column<string>(nullable: false),
                    MetadataValue = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadata", x => x.RecordMetadataID);
                    table.ForeignKey(
                        name: "FK_Metadata_Records_RecordID",
                        column: x => x.RecordID,
                        principalTable: "Records",
                        principalColumn: "RecordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationValues_ValueCollectionGuid",
                table: "ConfigurationValues",
                column: "ValueCollectionGuid");

            migrationBuilder.CreateIndex(
                name: "IX_GameRecordsConfigurationProfiles_GameID",
                table: "GameRecordsConfigurationProfiles",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_GameRecordsConfigurationProfiles_ProfileID",
                table: "GameRecordsConfigurationProfiles",
                column: "ProfileID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_RecordID",
                table: "Metadata",
                column: "RecordID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationValues");

            migrationBuilder.DropTable(
                name: "GameRecordsConfigurationProfiles");

            migrationBuilder.DropTable(
                name: "MappedControllerElements");

            migrationBuilder.DropTable(
                name: "Metadata");

            migrationBuilder.DropTable(
                name: "ConfigurationProfiles");

            migrationBuilder.DropTable(
                name: "ControllerElementMappings");

            migrationBuilder.DropTable(
                name: "Records");
        }
    }
}
