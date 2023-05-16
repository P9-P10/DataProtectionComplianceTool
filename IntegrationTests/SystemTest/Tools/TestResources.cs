using System.Data;
using System.Globalization;
using Dapper;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace IntegrationTests.SystemTest.Tools;

public class TestResources
{
    protected const string Description = "This is a description";
    protected const string Condition = "TRUE";

    protected const string IndividualsTable = "people";
    protected const string IndividualsColumn = "Id";
    protected static readonly TableColumnPair IndividualsSource = new(IndividualsTable, IndividualsColumn);

    protected static readonly StorageRule TestStorageRule = new()
    {
        Key = "deleteConditionName",
        Description = Description,
        VacuumingCondition = Condition
    };

    protected static readonly StorageRule TestNewTestStorageRule = new()
    {
        Key = TestStorageRule.Key + "NEW",
        Description = TestStorageRule.Description + "NEW",
        VacuumingCondition = TestStorageRule.VacuumingCondition + "NEW"
    };

    protected static readonly Purpose TestPurpose = new()
    {
        Key = "purposeName",
        Description = Description,
        StorageRules = new List<StorageRule>() {TestStorageRule},
        LegallyRequired = true,
        Rules = new List<VacuumingRule>() { }
    };

    protected static readonly Purpose NewTestPurpose = new()
    {
        Key = TestPurpose.Key + "NEW",
        Description = TestPurpose.Description + "NEW",
        StorageRules = new List<StorageRule>() {TestNewTestStorageRule},
        LegallyRequired = !TestPurpose.LegallyRequired,
        Rules = new List<VacuumingRule>()
    };

    protected static readonly Purpose NewTestPurposeWithOldRule = new Purpose()
    {
        Key = TestPurpose.Key + "NEW",
        Description = TestPurpose.Description + "NEW",
        StorageRules = new List<StorageRule>() {TestStorageRule},
        LegallyRequired = !TestPurpose.LegallyRequired,
        Rules = new List<VacuumingRule>()
    };

    protected static readonly Purpose VeryNewTestPurpose = new()
    {
        Key = TestPurpose.Key + "VERY_NEW",
        Description = TestPurpose.Description + "VERY_NEW",
        StorageRules = new List<StorageRule>() {TestNewTestStorageRule},
        LegallyRequired = !NewTestPurpose.LegallyRequired,
        Rules = new List<VacuumingRule>()
    };

    protected static readonly PersonalDataColumn TestPersonalDataColumn = new()
    {
        Key = new TableColumnPair("TestTable", "TestColumn"),
        Purposes = new[] {TestPurpose},
        DefaultValue = "testDefaultValue",
        Description = Description
    };

    protected static readonly PersonalDataColumn UpdatedTestPersonalDataColumn = new()
    {
        Key = TestPersonalDataColumn.Key,
        Purposes = TestPersonalDataColumn.Purposes,
        DefaultValue = TestPersonalDataColumn.DefaultValue + "UPDATED",
        Description = TestPersonalDataColumn.Description + "UPDATED",
    };

    protected static readonly PersonalDataColumn TestPersonalDataColumnWithMorePurposes = new PersonalDataColumn()
    {
        Key = TestPersonalDataColumn.Key,
        Purposes = new []{TestPurpose,NewTestPurpose,VeryNewTestPurpose},
        Description = TestPersonalDataColumn.Description,
        DefaultValue = TestPersonalDataColumn.DefaultValue
    };

    protected static readonly PersonalDataColumn NewTestPersonalDataColumn = new()
    {
        Key = new TableColumnPair(
            TestPersonalDataColumn.Key.TableName + "NEW",
            TestPersonalDataColumn.Key.ColumnName + "NEW"),
        Purposes = TestPersonalDataColumn.Purposes,
        DefaultValue = TestPersonalDataColumn.DefaultValue + "NEW",
        Description = TestPersonalDataColumn.Description + "NEW"
    };

    protected static readonly Processing TestProcessing = new()
    {
        Key = "ProcessingName", Description = "ProcessingDescription",
        PersonalDataColumn = TestPersonalDataColumn,
        Purpose = TestPurpose
    };

    protected static readonly Processing NewTestProcessing = new()
    {
        Key = "NewProcessingName", Description = "NewProcessingDescription",
        PersonalDataColumn = TestPersonalDataColumn,
        Purpose = TestPurpose
    };

    protected static readonly Origin TestOrigin = new()
    {
        Key = "originName",
        Description = Description,
        PersonalDataColumns = new List<PersonalDataColumn>()
    };

    protected static readonly Individual TestIndividual1 = new()
    {
        Id = 1,
        Key = 1
    };

    protected static readonly Individual TestIndividual2 = new()
    {
        Id = 2,
        Key = 2
    };

    protected static readonly Individual TestIndividual3 = new()
    {
        Id = 3,
        Key = 3
    };

    protected static readonly VacuumingRule TestVacuumingRule = new()
    {
        Key = "vacuumingRule",
        Description = Description,
        Interval = "2h 4d",
        Purposes = new List<Purpose> {TestPurpose}
    };

    protected static readonly VacuumingRule UpdatedTestVacuumingRule = new()
    {
        Key = TestVacuumingRule.Key + "UPDATED",
        Description = TestVacuumingRule.Description + "NEW",
        Interval = TestVacuumingRule.Interval + "6h",
        Purposes = new List<Purpose> {TestPurpose}
    };

    protected static void AddDeleteCondition(TestProcess testProcess, StorageRule storageRule)
    {
        var addDeleteConditionCommand = $"{CommandNamer.DeleteConditionsName} {CommandNamer.Create} " +
                                        $"{OptionNamer.Name} {storageRule.Key} " +
                                        $"{OptionNamer.Condition} \"{storageRule.VacuumingCondition}\" " +
                                        $"{OptionNamer.Description} \"{storageRule.Description}\"";

        testProcess.GiveInput(addDeleteConditionCommand);
    }

    protected static void AddPurpose(TestProcess testProcess, Purpose purpose)
    {
        var addPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Create} " +
                                $"{OptionNamer.Name} {purpose.Key} " +
                                $"{OptionNamer.Description} \"{purpose.Description}\" " +
                                $"{OptionNamer.DeleteConditionName} {String.Join(" ", purpose.StorageRules.Select(p => p.Key).ToList())} " +
                                $"{OptionNamer.LegallyRequired} {purpose.LegallyRequired} ";

        testProcess.GiveInput(addPurposeCommand);
    }

    protected static void ShowPurpose(TestProcess testProcess, Purpose purpose)
    {
        var showPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Show} " +
                                 $"{OptionNamer.Name} {purpose.Key}";

        testProcess.GiveInput(showPurposeCommand);
    }

    protected static void UpdatePurpose(TestProcess testProcess, Purpose old, Purpose updated)
    {
        var updatePurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Update} " +
                                   $"{OptionNamer.Name} {old.Key} " +
                                   $"{OptionNamer.Description} \"{updated.Description}\" " +
                                   $"{OptionNamer.LegallyRequired} {updated.LegallyRequired} " +
                                   $"{OptionNamer.NewName} {updated.Key}";

        testProcess.GiveInput(updatePurposeCommand);
    }

    protected static void ListPurpose(TestProcess testProcess)
    {
        testProcess.GiveInput($"{CommandNamer.PurposesName} {CommandNamer.List}");
    }

    protected static void DeletePurpose(TestProcess testProcess, Purpose purpose)
    {
        var deleteCommand = $"{CommandNamer.PurposesName} {CommandNamer.Delete} " +
                            $"{OptionNamer.Name} {purpose.Key}";
        testProcess.GiveInput(deleteCommand);
    }

    protected static void ListDeletionConditions(TestProcess process)
    {
        process.GiveInput($"{CommandNamer.DeleteConditionsAlias} {CommandNamer.List}");
    }

    protected static void UpdateDeletionCondition(TestProcess process, StorageRule old,
        StorageRule newDeletionCondition)
    {
        string command =
            $"{CommandNamer.DeleteConditionsAlias} {CommandNamer.UpdateAlias} {OptionNamer.NameAlias} {old.Key}  {OptionNamer.NewNameAlias}  {newDeletionCondition.Key}" +
            $" {OptionNamer.Condition} \"{newDeletionCondition.VacuumingCondition}\"" +
            $" {OptionNamer.Description} \"{newDeletionCondition.Description}\"";
        process.GiveInput(command);
    }

    protected static void DeleteDeletionCondition(TestProcess process, StorageRule storageRule)
    {
        process.GiveInput($"{CommandNamer.DeleteConditionsAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name}" +
                          $" {storageRule.Key}");
    }

    protected static void ShowDeleteCondition(TestProcess process, StorageRule storageRule)
    {
        process.GiveInput(
            $"{CommandNamer.DeleteConditionsAlias} {CommandNamer.ShowAlias} -n {storageRule.Key}");
    }

    protected static void AddOrigin(TestProcess testProcess, Origin origin)
    {
        string command =
            $"{CommandNamer.OriginsAlias} {CommandNamer.CreateAlias} {OptionNamer.Name} {origin.Key} {OptionNamer.Description} \"{origin.Description}\"";
        testProcess.GiveInput(command);
    }

    protected static void ListOrigins(TestProcess process)
    {
        process.GiveInput($"{CommandNamer.OriginsAlias} {CommandNamer.List}");
    }

    protected static void UpdateOrigin(TestProcess process, Origin old, Origin newOrigin)
    {
        string command = $"{CommandNamer.OriginsAlias} {CommandNamer.UpdateAlias} {OptionNamer.Name} " +
                         $"{old.Key} {OptionNamer.NewName} {newOrigin.Key} {OptionNamer.Description} \"{newOrigin.Description}\"";
        process.GiveInput(command);
    }

    protected static void DeleteOrigin(TestProcess process, Origin origin)
    {
        process.GiveInput(
            $"{CommandNamer.OriginsAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name} {origin.Key}");
    }

    protected static void ShowOrigin(TestProcess process, Origin origin)
    {
        process.GiveInput(
            $"{CommandNamer.OriginsAlias} {CommandNamer.ShowAlias} {OptionNamer.Name} {origin.Key}");
    }

    protected static void AddProcessing(TestProcess process, Processing processing)
    {
        string command =
            $"{CommandNamer.ProcessingsName} {CommandNamer.Create} {OptionNamer.Name} {processing.Key}" +
            $" {OptionNamer.Description} {processing.Description}" +
            $" {OptionNamer.TableColumn} {processing.PersonalDataColumn.Key.TableName} {processing.PersonalDataColumn.Key.ColumnName}" +
            $" {OptionNamer.Purpose} {processing.Purpose.Key}";
        process.GiveInput(command);
    }

    protected static void ListProcessing(TestProcess process)
    {
        string command = $"{CommandNamer.ProcessingsAlias} {CommandNamer.List}";
        process.GiveInput(command);
    }

    protected static void UpdateProcessing(TestProcess process, Processing old, Processing newProcess)
    {
        string command =
            $"{CommandNamer.ProcessingsAlias} {CommandNamer.UpdateAlias} {OptionNamer.Name} {old.Key}" +
            $" {OptionNamer.NewNameAlias} {newProcess.Key}" +
            $" {OptionNamer.Description} {newProcess.Description}";
        process.GiveInput(command);
    }

    protected static void DeleteProcessing(TestProcess process, Processing processing)
    {
        string command =
            $"{CommandNamer.ProcessingsAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name} {processing.Key}";
        process.GiveInput(command);
    }

    protected static void ShowProcessing(TestProcess process, Processing processing)
    {
        string command =
            $"{CommandNamer.ProcessingsAlias} {CommandNamer.ShowAlias} {OptionNamer.Name} {processing.Key}";
        process.GiveInput(command);
    }

    protected static void AddPersonalData(TestProcess process, PersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.CreateAlias}" +
                      $" {OptionNamer.TableColumn} {personalDataColumn.Key.TableName} {personalDataColumn.Key.ColumnName}" +
                      $" {OptionNamer.DefaultValueAlias} \"{personalDataColumn.DefaultValue}\"" +
                      $" {OptionNamer.Description} \"{personalDataColumn.Description}\"" +
                      $" {OptionNamer.PurposeList} {string.Join(" ", personalDataColumn.Purposes.Select(p => p.Key))}";
        process.GiveInput(command);
    }

    protected static void UpdatePersonalData(TestProcess testProcess, PersonalDataColumn old,
        PersonalDataColumn updated)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.Update} " +
                      $"{OptionNamer.TableColumn} {old.Key.TableName} {old.Key.ColumnName} " +
                      $"{OptionNamer.DefaultValue} \"{updated.DefaultValue}\" " +
                      $"{OptionNamer.Description} \"{updated.Description}\" ";

        testProcess.GiveInput(command);
    }

    protected static void ShowPersonalData(TestProcess testProcess, PersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.Show} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} ";

        testProcess.GiveInput(command);
    }

    protected static void ListPersonalData(TestProcess testProcess)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.List}";
        testProcess.GiveInput(command);
    }

    protected static void DeletePersonalData(TestProcess testProcess, PersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.Delete} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} ";
        testProcess.GiveInput(command);
    }

    protected static void AddPurposesToPersonalData(TestProcess testProcess, PersonalDataColumn personalDataColumn,
        IEnumerable<Purpose> purposes)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.AddPurpose} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void RemovePurposesFromPersonalData(TestProcess testProcess,
        PersonalDataColumn personalDataColumn,
        IEnumerable<Purpose> purposes)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.RemovePurpose} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void SetOriginOfPersonalData(TestProcess testProcess, PersonalDataColumn personalDataColumn,
        Individual individual, Origin origin)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.SetOriginAlias} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.Id} {individual.ToListing()} " +
                      $"{OptionNamer.Origin} {origin.Key}";

        testProcess.GiveInput(command);
    }

    protected static void ShowOriginOfPersonalData(TestProcess testProcess, PersonalDataColumn personalDataColumn,
        Individual individual)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.ShowOrigin} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.Id} {individual.ToListing()} ";

        testProcess.GiveInput(command);
    }

    protected static void SetIndividualsSource(TestProcess testProcess, TableColumnPair source)
    {
        var command = $"{CommandNamer.IndividualsName} {CommandNamer.SetSource} " +
                      $"{OptionNamer.TableColumn} {source.TableName} {source.ColumnName}";
        testProcess.GiveInput(command);
    }

    protected static void ShowIndividualsSource(TestProcess testProcess)
    {
        var command = $"{CommandNamer.IndividualsName} {CommandNamer.ShowSource}";
        testProcess.GiveInput(command);
    }

    protected static void ListIndividuals(TestProcess testProcess)
    {
        var command = $"{CommandNamer.IndividualsName} {CommandNamer.List}";
        testProcess.GiveInput(command);
    }

    protected static void AddVacuumingRule(TestProcess testProcess, VacuumingRule vacuumingRule)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Create} " +
                      $"{OptionNamer.Name} {vacuumingRule.Key} " +
                      $"{OptionNamer.Interval} \"{vacuumingRule.Interval}\" " +
                      $"{OptionNamer.PurposeListAlias} {vacuumingRule.Purposes.First().ToListingIdentifier()} " +
                      $"{OptionNamer.Description} \"{vacuumingRule.Description}\"";
        testProcess.GiveInput(command);
    }

    protected static void UpdateVacuumingRule(TestProcess testProcess, VacuumingRule old, VacuumingRule updated)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Update} " +
                      $"{OptionNamer.Name} {old.Key} " +
                      $"{OptionNamer.NewName} {updated.Key} " +
                      $"{OptionNamer.Interval} \"{updated.Interval}\" " +
                      $"{OptionNamer.Description} \"{updated.Description}\"";
        testProcess.GiveInput(command);
    }

    protected static void ShowVacuumingRule(TestProcess testProcess, VacuumingRule vacuumingRule)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Show} " +
                      $"{OptionNamer.Name} {vacuumingRule.Key}";
        testProcess.GiveInput(command);
    }

    protected static void ListVacuumingRule(TestProcess testProcess)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.List}";
        testProcess.GiveInput(command);
    }

    protected static void DeleteVacuumingRule(TestProcess testProcess, VacuumingRule vacuumingRule)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Delete} " +
                      $"{OptionNamer.Name} {vacuumingRule.Key}";
        testProcess.GiveInput(command);
    }

    protected static void ExecuteVacuumingRule(TestProcess testProcess, IEnumerable<VacuumingRule> vacuumingRules)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Execute} " +
                      $"{OptionNamer.Rules} {string.Join(" ", vacuumingRules.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void AddPurposesToVacuumingRule(TestProcess testProcess, VacuumingRule vacuumingRule,
        IEnumerable<Purpose> purposes)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.AddPurpose} " +
                      $"{OptionNamer.Name} {vacuumingRule.Key} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void RemovePurposesFromVacuumingRule(TestProcess testProcess, VacuumingRule vacuumingRule,
        IEnumerable<Purpose> purposes)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.RemovePurpose} " +
                      $"{OptionNamer.Name} {vacuumingRule.Key} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void ListLogs(TestProcess testProcess, LogConstraints constraints)
    {
        // ReSharper disable once StringLiteralTypo
        const string dateTimeFormat = "yyyy/MM/dd'T'HH:mm:ss";
        var dateTimeStart = constraints.LogTimeRange.Start.ToString(dateTimeFormat, CultureInfo.InvariantCulture);
        var dateTimeEnd = constraints.LogTimeRange.End.ToString(dateTimeFormat, CultureInfo.InvariantCulture);

        var command = $"{CommandNamer.LoggingName} {CommandNamer.List} " +
                      $"{OptionNamer.Limit} {constraints.Limit} " +
                      $"{OptionNamer.Numbers} {constraints.LogNumbersRange.Start} {constraints.LogNumbersRange.End} " +
                      $"{OptionNamer.DateTimes} {dateTimeStart} {dateTimeEnd} " +
                      $"{OptionNamer.LogTypes} {string.Join(" ", constraints.LogTypes.Select(lt => lt.ToString()))} " +
                      $"{OptionNamer.LogFormats} {string.Join(" ", constraints.LogMessageFormats.Select(lf => lf.ToString()))} " +
                      (constraints.Subjects.Any()
                          ? $"{OptionNamer.Subjects} {string.Join(" ", constraints.Subjects.Select(s => $"\"{s}\""))}"
                          : "");
        testProcess.GiveInput(command);
    }

    protected static void AddLogEntryOrigin(TestProcess testProcess, string name)
    {
        var command = $"{CommandNamer.OriginsName} {CommandNamer.Create} {OptionNamer.Name} {name}";
        testProcess.GiveInput(command);
    }

    protected static void InsertIndividual(IDbConnection dbConnection, Individual individual)
    {
        dbConnection.Execute(
            $"INSERT INTO {IndividualsTable} ({IndividualsColumn}, Key) VALUES ({individual.Id}, {individual.Key})");
    }

    protected static void CreatePersonalDataTable(IDbConnection dbConnection, PersonalDataColumn personalDataColumn)
    {
        var createTable = @$"CREATE TABLE IF NOT EXISTS {personalDataColumn.Key.TableName} (
            Id INTEGER PRIMARY KEY,
            {personalDataColumn.Key.ColumnName} VARCHAR
        );";

        dbConnection.Execute(createTable);
    }

    protected static void InsertPersonalData(IDbConnection dbConnection, PersonalDataColumn personalDataColumn,
        Individual individual, string value)
    {
        var query = $"INSERT INTO " +
                    $"{personalDataColumn.Key.TableName} (Id, {personalDataColumn.Key.ColumnName}) " +
                    $"VALUES ({individual.Id}, '{value}')";
        dbConnection.Execute(query);
    }

    protected static void SetupTestData(IDbConnection dbConnection)
    {
        InsertIndividual(dbConnection, TestIndividual1);
        InsertIndividual(dbConnection, TestIndividual2);
        InsertIndividual(dbConnection, TestIndividual3);

        CreatePersonalDataTable(dbConnection, TestPersonalDataColumn);

        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual1, $"{TestIndividual1.Id}");
        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual2, $"{TestIndividual2.Id}");
        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual3, $"{TestIndividual3.Id}");
    }
}