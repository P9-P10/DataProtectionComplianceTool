using System.Data;
using System.Globalization;
using Dapper;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Archive;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace IntegrationTests.SystemTest.Tools;

public class TestResources
{
    protected const string Description = "This is a description";
    protected const string Condition = "TRUE";
    
    protected const string IndividualsTable = "people";
    protected const string IndividualsColumn = "Id";
    protected static readonly TableColumnPair IndividualsSource = new(IndividualsTable, IndividualsColumn);

    protected static readonly DeleteCondition TestDeleteCondition = new()
    {
        Name = "deleteConditionName",
        Description = Description,
        Condition = Condition
    };

    protected static readonly DeleteCondition NewTestDeleteCondition = new()
    {
        Name = TestDeleteCondition.GetName() + "NEW",
        Description = TestDeleteCondition.GetDescription() + "NEW",
        Condition = TestDeleteCondition.GetCondition() + "NEW"
    };

    protected static readonly Purpose TestPurpose = new()
    {
        Name = "purposeName",
        Description = Description,
        DeleteConditions = TestDeleteCondition,
        LegallyRequired = true,
        PersonalDataColumns = new List<PersonalDataColumn>(),
        Rules = new List<VacuumingRule>()
    };

    protected static readonly Purpose NewTestPurpose = new()
    {
        Name = TestPurpose.GetName() + "NEW",
        Description = TestPurpose.GetDescription() + "NEW",
        DeleteConditions = NewTestDeleteCondition,
        LegallyRequired = !TestPurpose.GetLegallyRequired(),
        PersonalDataColumns = new List<PersonalDataColumn>(),
        Rules = new List<VacuumingRule>()
    };

    protected static readonly Purpose VeryNewTestPurpose = new()
    {
        Name = TestPurpose.GetName() + "VERY_NEW",
        Description = TestPurpose.GetDescription() + "VERY_NEW",
        DeleteConditions = NewTestDeleteCondition,
        LegallyRequired = !NewTestPurpose.GetLegallyRequired(),
        PersonalDataColumns = new List<PersonalDataColumn>(),
        Rules = new List<VacuumingRule>()
    };

    protected static readonly PersonalDataColumn TestPersonalDataColumn = new()
    {
        TableColumnPair = new TableColumnPair("TestTable", "TestColumn"),
        Purposes = new[] { TestPurpose },
        DefaultValue = "testDefaultValue",
        Description = Description,
        JoinCondition = "This is a join condition"
    };

    protected static readonly PersonalDataColumn UpdatedTestPersonalDataColumn = new()
    {
        TableColumnPair = TestPersonalDataColumn.TableColumnPair,
        Purposes = TestPersonalDataColumn.Purposes,
        DefaultValue = TestPersonalDataColumn.DefaultValue + "UPDATED",
        Description = TestPersonalDataColumn.Description + "UPDATED",
        JoinCondition = TestPersonalDataColumn.JoinCondition
    };
    
    protected static readonly PersonalDataColumn NewTestPersonalDataColumn = new()
    {
        TableColumnPair = new TableColumnPair(
            TestPersonalDataColumn.TableColumnPair.TableName + "NEW",
            TestPersonalDataColumn.TableColumnPair.ColumnName + "NEW"),
        Purposes = TestPersonalDataColumn.Purposes,
        DefaultValue = TestPersonalDataColumn.DefaultValue + "NEW",
        Description = TestPersonalDataColumn.Description + "NEW",
        JoinCondition = TestPersonalDataColumn.JoinCondition + "NEW"
    };

    protected static readonly Processing TestProcessing = new()
    {
        Name = "ProcessingName", Description = "ProcessingDescription",
        PersonalDataColumn = TestPersonalDataColumn,
        Purpose = TestPurpose
    };
    
    protected static readonly Processing NewTestProcessing = new()
    {
        Name = "NewProcessingName", Description = "NewProcessingDescription",
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
        Id = 1
    };
    
    protected static readonly Individual TestIndividual2 = new()
    {
        Id = 2
    };
    
    protected static readonly Individual TestIndividual3 = new()
    {
        Id = 3
    };

    protected static readonly VacuumingRule TestVacuumingRule = new()
    {
        Name = "vacuumingRule",
        Description = Description,
        Interval = "2h 4d",
        Purposes = new List<Purpose> { TestPurpose }
    };

    protected static readonly VacuumingRule UpdatedTestVacuumingRule = new()
    {
        Name = TestVacuumingRule.GetName() + "UPDATED",
        Description = TestVacuumingRule.GetDescription() + "NEW",
        Interval = TestVacuumingRule.GetInterval() + "6h",
        Purposes = new List<Purpose> { TestPurpose }
    };

    protected static void AddDeleteCondition(TestProcess testProcess, IDeleteCondition deleteCondition)
    {
        var addDeleteConditionCommand = $"{CommandNamer.DeleteConditionsName} {CommandNamer.Create} " +
                                        $"{OptionNamer.Name} {deleteCondition.GetName()} " +
                                        $"{OptionNamer.Condition} \"{deleteCondition.GetCondition()}\" " +
                                        $"{OptionNamer.Description} \"{deleteCondition.GetDescription()}\"";

        testProcess.GiveInput(addDeleteConditionCommand);
    }

    protected static void AddPurpose(TestProcess testProcess, IPurpose purpose)
    {
        var addPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Create} " +
                                $"{OptionNamer.Name} {purpose.GetName()} " +
                                $"{OptionNamer.Description} \"{purpose.GetDescription()}\" " +
                                $"{OptionNamer.DeleteConditionName} {purpose.GetDeleteCondition()} " +
                                $"{OptionNamer.LegallyRequired} {purpose.GetLegallyRequired()} ";

        testProcess.GiveInput(addPurposeCommand);
    }

    protected static void ShowPurpose(TestProcess testProcess, IPurpose purpose)
    {
        var showPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Show} " +
                                 $"{OptionNamer.Name} {purpose.GetName()}";

        testProcess.GiveInput(showPurposeCommand);
    }

    protected static void UpdatePurpose(TestProcess testProcess, IPurpose old, IPurpose updated)
    {
        var updatePurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Update} " +
                                   $"{OptionNamer.Name} {old.GetName()} " +
                                   $"{OptionNamer.Description} \"{updated.GetDescription()}\" " +
                                   $"{OptionNamer.LegallyRequired} {updated.GetLegallyRequired()} " +
                                   $"{OptionNamer.DeleteConditionName} {updated.GetDeleteCondition()} " +
                                   $"{OptionNamer.NewName} {updated.GetName()}";

        testProcess.GiveInput(updatePurposeCommand);
    }

    protected static void ListPurpose(TestProcess testProcess)
    {
        testProcess.GiveInput($"{CommandNamer.PurposesName} {CommandNamer.List}");
    }

    protected static void DeletePurpose(TestProcess testProcess, IPurpose purpose)
    {
        var deleteCommand = $"{CommandNamer.PurposesName} {CommandNamer.Delete} " +
                            $"{OptionNamer.Name} {purpose.GetName()}";
        testProcess.GiveInput(deleteCommand);
    }

    protected static void ListDeletionConditions(TestProcess process)
    {
        process.GiveInput($"{CommandNamer.DeleteConditionsAlias} {CommandNamer.List}");
    }

    protected static void UpdateDeletionCondition(TestProcess process, IDeleteCondition old,
        IDeleteCondition newDeletionCondition)
    {
        string command =
            $"{CommandNamer.DeleteConditionsAlias} {CommandNamer.UpdateAlias} {OptionNamer.NameAlias} {old.GetName()}  {OptionNamer.NewNameAlias}  {newDeletionCondition.GetName()}" +
            $" {OptionNamer.Condition} \"{newDeletionCondition.GetCondition()}\"" +
            $" {OptionNamer.Description} \"{newDeletionCondition.GetDescription()}\"";
        process.GiveInput(command);
    }

    protected static void DeleteDeletionCondition(TestProcess process, IDeleteCondition deleteCondition)
    {
        process.GiveInput($"{CommandNamer.DeleteConditionsAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name}" +
                          $" {deleteCondition.GetName()}");
    }

    protected static void ShowDeleteCondition(TestProcess process, IDeleteCondition deleteCondition)
    {
        process.GiveInput(
            $"{CommandNamer.DeleteConditionsAlias} {CommandNamer.ShowAlias} -n {deleteCondition.GetName()}");
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

    protected static void AddProcessing(TestProcess process, IProcessing processing)
    {
        string command =
            $"{CommandNamer.ProcessingsName} {CommandNamer.Create} {OptionNamer.Name} {processing.GetName()}" +
            $" {OptionNamer.Description} {processing.GetDescription()}" +
            $" {OptionNamer.TableColumn} {processing.GetPersonalDataTableColumnPair().TableName} {processing.GetPersonalDataTableColumnPair().ColumnName}" +
            $" {OptionNamer.Purpose} {processing.GetPurpose().GetName()}";
        process.GiveInput(command);
    }

    protected static void ListProcessing(TestProcess process)
    {
        string command = $"{CommandNamer.ProcessingsAlias} {CommandNamer.List}";
        process.GiveInput(command);
    }

    protected static void UpdateProcessing(TestProcess process, IProcessing old, IProcessing newProcess)
    {
        string command = $"{CommandNamer.ProcessingsAlias} {CommandNamer.UpdateAlias} {OptionNamer.Name} {old.GetName()}" +
                         $" {OptionNamer.NewNameAlias} {newProcess.GetName()}" +
                         $" {OptionNamer.Description} {newProcess.GetDescription()}";
        process.GiveInput(command);
    }

    protected static void DeleteProcessing(TestProcess process, IProcessing processing)
    {
        string command = $"{CommandNamer.ProcessingsAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name} {processing.GetName()}";
        process.GiveInput(command);
    }

    protected static void ShowProcessing(TestProcess process, IProcessing processing)
    {
        string command = $"{CommandNamer.ProcessingsAlias} {CommandNamer.ShowAlias} {OptionNamer.Name} {processing.GetName()}";
        process.GiveInput(command);
    }

    protected static void AddPersonalData(TestProcess process, IPersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnsAlias} {CommandNamer.CreateAlias}" +
                      $" {OptionNamer.TableColumn} {personalDataColumn.GetTableColumnPair().TableName} {personalDataColumn.GetTableColumnPair().ColumnName}" +
                      $" {OptionNamer.JoinCondition} \"{personalDataColumn.GetJoinCondition()}\"" +
                      $" {OptionNamer.DefaultValueAlias} \"{personalDataColumn.GetDefaultValue()}\"" +
                      $" {OptionNamer.Description} \"{personalDataColumn.GetDescription()}\"" +
                      $" {OptionNamer.PurposeList} {string.Join(" ", personalDataColumn.GetPurposes().Select(p => p.GetName()))}";
        process.GiveInput(command);
    }

    protected static void UpdatePersonalData(TestProcess testProcess, IPersonalDataColumn old,
        IPersonalDataColumn updated)
    {
        var command = $"{CommandNamer.PersonalDataColumnsName} {CommandNamer.Update} " +
                      $"{OptionNamer.TableColumn} {old.GetTableColumnPair().TableName} {old.GetTableColumnPair().ColumnName} " +
                      $"{OptionNamer.DefaultValue} \"{updated.GetDefaultValue()}\" " +
                      $"{OptionNamer.Description} \"{updated.GetDescription()}\" ";

        testProcess.GiveInput(command);
    }

    protected static void ShowPersonalData(TestProcess testProcess, IPersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnsName} {CommandNamer.Show} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.GetTableColumnPair().TableName} " +
                      $"{personalDataColumn.GetTableColumnPair().ColumnName} ";

        testProcess.GiveInput(command);
    }

    protected static void ListPersonalData(TestProcess testProcess)
    {
        var command = $"{CommandNamer.PersonalDataColumnsName} {CommandNamer.List}";
        testProcess.GiveInput(command);
    }

    protected static void DeletePersonalData(TestProcess testProcess, IPersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnsName} {CommandNamer.Delete} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.GetTableColumnPair().TableName} " +
                      $"{personalDataColumn.GetTableColumnPair().ColumnName} ";
        testProcess.GiveInput(command);
    }

    protected static void AddPurposesToPersonalData(TestProcess testProcess, IPersonalDataColumn personalDataColumn,
        IEnumerable<IPurpose> purposes)
    {
        var command = $"{CommandNamer.PersonalDataColumnsName} {CommandNamer.AddPurpose} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.GetTableColumnPair().TableName} " +
                      $"{personalDataColumn.GetTableColumnPair().ColumnName} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.GetName()))}";
        testProcess.GiveInput(command);
    }

    protected static void RemovePurposesFromPersonalData(TestProcess testProcess, IPersonalDataColumn personalDataColumn,
        IEnumerable<IPurpose> purposes)
    {
        var command = $"{CommandNamer.PersonalDataColumnsName} {CommandNamer.RemovePurpose} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.GetTableColumnPair().TableName} " +
                      $"{personalDataColumn.GetTableColumnPair().ColumnName} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.GetName()))}";
        testProcess.GiveInput(command);
    }

    protected static void SetOriginOfPersonalData(TestProcess testProcess, IPersonalDataColumn personalDataColumn,
        IIndividual individual, Origin origin)
    {
        var command = $"{CommandNamer.PersonalDataColumnsName} {CommandNamer.SetOrigin} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.GetTableColumnPair().TableName} " +
                      $"{personalDataColumn.GetTableColumnPair().ColumnName} " +
                      $"{OptionNamer.Id} {individual.ToListing()} " +
                      $"{OptionNamer.Origin} {origin.Key}";

        testProcess.GiveInput(command);
    }

    protected static void ShowOriginOfPersonalData(TestProcess testProcess, IPersonalDataColumn personalDataColumn,
        IIndividual individual)
    {
        var command = $"{CommandNamer.PersonalDataColumnsName} {CommandNamer.ShowOrigin} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.GetTableColumnPair().TableName} " +
                      $"{personalDataColumn.GetTableColumnPair().ColumnName} " +
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

    protected static void AddVacuumingRule(TestProcess testProcess, IVacuumingRule vacuumingRule)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Create} " +
                      $"{OptionNamer.Name} {vacuumingRule.GetName()} " +
                      $"{OptionNamer.Interval} \"{vacuumingRule.GetInterval()}\" " +
                      $"{OptionNamer.Purpose} {vacuumingRule.GetPurposes().First().ToListingIdentifier()} " +
                      $"{OptionNamer.Description} \"{vacuumingRule.GetDescription()}\"";
        testProcess.GiveInput(command);
    }

    protected static void UpdateVacuumingRule(TestProcess testProcess, IVacuumingRule old, IVacuumingRule updated)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Update} " +
                      $"{OptionNamer.Name} {old.GetName()} " +
                      $"{OptionNamer.NewName} {updated.GetName()} " +
                      $"{OptionNamer.Interval} \"{updated.GetInterval()}\" " +
                      $"{OptionNamer.Description} \"{updated.GetDescription()}\"";
        testProcess.GiveInput(command);
    }

    protected static void ShowVacuumingRule(TestProcess testProcess, IVacuumingRule vacuumingRule)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Show} " +
                      $"{OptionNamer.Name} {vacuumingRule.GetName()}";
        testProcess.GiveInput(command);
    }

    protected static void ListVacuumingRule(TestProcess testProcess)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.List}";
        testProcess.GiveInput(command);
    }

    protected static void DeleteVacuumingRule(TestProcess testProcess, IVacuumingRule vacuumingRule)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Delete} " +
                      $"{OptionNamer.Name} {vacuumingRule.GetName()}";
        testProcess.GiveInput(command);
    }

    protected static void ExecuteVacuumingRule(TestProcess testProcess, IEnumerable<IVacuumingRule> vacuumingRules)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.Execute} " +
                      $"{OptionNamer.Rules} {string.Join(" ", vacuumingRules.Select(p => p.GetName()))}";
        testProcess.GiveInput(command);
    }

    protected static void AddPurposesToVacuumingRule(TestProcess testProcess, IVacuumingRule vacuumingRule,
        IEnumerable<IPurpose> purposes)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.AddPurpose} " +
                      $"{OptionNamer.Name} {vacuumingRule.GetName()} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.GetName()))}";
        testProcess.GiveInput(command);
    }

    protected static void RemovePurposesFromVacuumingRule(TestProcess testProcess, IVacuumingRule vacuumingRule,
        IEnumerable<IPurpose> purposes)
    {
        var command = $"{CommandNamer.VacuumingRulesName} {CommandNamer.RemovePurpose} " +
                       $"{OptionNamer.Name} {vacuumingRule.GetName()} " +
                       $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.GetName()))}";
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

    protected static void InsertIndividual(IDbConnection dbConnection, IIndividual individual)
    {
        dbConnection.Execute($"INSERT INTO {IndividualsTable} ({IndividualsColumn}) VALUES ({individual.ToListing()})");
    }
    
    protected static void CreatePersonalDataTable(IDbConnection dbConnection, IPersonalDataColumn personalDataColumn)
    {
        var createTable = @$"CREATE TABLE IF NOT EXISTS {personalDataColumn.GetTableColumnPair().TableName} (
            Id INTEGER PRIMARY KEY,
            {personalDataColumn.GetTableColumnPair().ColumnName} VARCHAR
        );";

        dbConnection.Execute(createTable);
    }
    
    protected static void InsertPersonalData(IDbConnection dbConnection, IPersonalDataColumn personalDataColumn, IIndividual individual, string value)
    {
        var query = $"INSERT INTO " +
                    $"{personalDataColumn.GetTableColumnPair().TableName} (Id, {personalDataColumn.GetTableColumnPair().ColumnName}) " +
                    $"VALUES ({individual.ToListing()}, '{value}')";
        dbConnection.Execute(query);
    }

    protected static void SetupTestData(IDbConnection dbConnection)
    {
        InsertIndividual(dbConnection, TestIndividual1);
        InsertIndividual(dbConnection, TestIndividual2);
        InsertIndividual(dbConnection, TestIndividual3);

        CreatePersonalDataTable(dbConnection, TestPersonalDataColumn);

        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual1, $"{TestIndividual1.ToListing()}");
        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual2, $"{TestIndividual2.ToListing()}");
        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual3, $"{TestIndividual3.ToListing()}");
    }
}
