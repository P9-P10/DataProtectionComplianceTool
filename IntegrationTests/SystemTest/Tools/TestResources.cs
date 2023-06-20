using System.Data;
using System.Globalization;
using Dapper;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace IntegrationTests.SystemTest.Tools;

public class TestResources
{
    protected const string Description = "This is a description";
    protected const string Condition = "TRUE";

    protected const string IndividualsTable = "people";
    protected const string IndividualsColumn = "Id";
    protected static readonly TableColumnPair IndividualsSource = new(IndividualsTable, IndividualsColumn);

    protected static readonly StoragePolicy StoragePolicy = new()
    {
        Key = "storagePolicyName",
        Description = Description,
        VacuumingCondition = Condition
    };

    protected static readonly StoragePolicy NewTestStoragePolicy = new()
    {
        Key = StoragePolicy.Key + "NEW",
        Description = StoragePolicy.Description + "NEW",
        VacuumingCondition = StoragePolicy.VacuumingCondition + "NEW"
    };

    protected static readonly LegalBasis TestLegalBasis = new()
    {
        Key = "testLegalBasis",
        Description = Description,
        PersonalDataColumns = new List<PersonalDataColumn>()
    };

    protected static readonly LegalBasis UpdatedTestLegalBasis = new()
    {
        Key = TestLegalBasis.Key + "UPDATED",
        Description = TestLegalBasis.Description + "UPDATED",
        PersonalDataColumns = TestLegalBasis.PersonalDataColumns
    };

    protected static readonly Purpose TestPurpose = new()
    {
        Key = "purposeName",
        Description = Description,
        StoragePolicies = new List<StoragePolicy>() { StoragePolicy },
        VacuumingPolicies = new List<VacuumingPolicy>() { }
    };

    protected static readonly Purpose NewTestPurpose = new()
    {
        Key = TestPurpose.Key + "NEW",
        Description = TestPurpose.Description + "NEW",
        StoragePolicies = new List<StoragePolicy>() { NewTestStoragePolicy },
        VacuumingPolicies = new List<VacuumingPolicy>()
    };

    protected static readonly Purpose NewTestPurposeWithOldPolicy = new()
    {
        Key = TestPurpose.Key + "NEW",
        Description = TestPurpose.Description + "NEW",
        StoragePolicies = new List<StoragePolicy>() { StoragePolicy },
        VacuumingPolicies = new List<VacuumingPolicy>()
    };

    protected static readonly Purpose VeryNewTestPurpose = new()
    {
        Key = TestPurpose.Key + "VERY_NEW",
        Description = TestPurpose.Description + "VERY_NEW",
        StoragePolicies = new List<StoragePolicy>() { NewTestStoragePolicy },
        VacuumingPolicies = new List<VacuumingPolicy>()
    };

    protected static readonly PersonalDataColumn TestPersonalDataColumn = new()
    {
        Key = new TableColumnPair("TestTable", "TestColumn"),
        Purposes = new[] { TestPurpose },
        LegalBases = new[] { TestLegalBasis },
        DefaultValue = "testDefaultValue",
        Description = Description,
        AssociationExpression = "This is a join condition"
    };

    protected static readonly PersonalDataColumn UpdatedTestPersonalDataColumn = new()
    {
        Key = TestPersonalDataColumn.Key,
        Purposes = TestPersonalDataColumn.Purposes,
        LegalBases = TestPersonalDataColumn.LegalBases,
        DefaultValue = TestPersonalDataColumn.DefaultValue + "UPDATED",
        Description = TestPersonalDataColumn.Description + "UPDATED",
        AssociationExpression = TestPersonalDataColumn.AssociationExpression + "UPDATED"
    };

    protected static readonly PersonalDataColumn NewTestPersonalDataColumn = new()
    {
        Key = new TableColumnPair(
            TestPersonalDataColumn.Key.TableName + "NEW",
            TestPersonalDataColumn.Key.ColumnName + "NEW"),
        Purposes = TestPersonalDataColumn.Purposes,
        LegalBases = TestPersonalDataColumn.LegalBases,
        DefaultValue = TestPersonalDataColumn.DefaultValue + "NEW",
        Description = TestPersonalDataColumn.Description + "NEW",
        AssociationExpression = TestPersonalDataColumn.AssociationExpression + "NEW"
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

    protected static readonly VacuumingPolicy VacuumingPolicy = new()
    {
        Key = "vacuumingPolicy",
        Description = Description,
        Duration = "2h 4d",
        Purposes = new List<Purpose> { TestPurpose }
    };

    protected static readonly VacuumingPolicy UpdatedTestVacuumingPolicy = new()
    {
        Key = VacuumingPolicy.Key + "UPDATED",
        Description = VacuumingPolicy.Description + "NEW",
        Duration = VacuumingPolicy.Duration + "6h",
        Purposes = new List<Purpose> { TestPurpose }
    };

    

    protected static void AddPurpose(TestProcess testProcess, Purpose purpose)
    {
        var addPurposeCommand = $"{CommandNamer.PurposeName} {CommandNamer.Create} " +
                                $"{OptionNamer.Name} {purpose.Key} " +
                                $"{OptionNamer.Description} \"{purpose.Description}\" " +
                                $"{OptionNamer.StoragePolicyList} {String.Join(" ", purpose.StoragePolicies.Select(p => p.Key).ToList())} ";

        testProcess.GiveInput(addPurposeCommand);
    }

    protected static void ShowPurpose(TestProcess testProcess, Purpose purpose)
    {
        var showPurposeCommand = $"{CommandNamer.PurposeName} {CommandNamer.Show} " +
                                 $"{OptionNamer.Name} {purpose.Key}";

        testProcess.GiveInput(showPurposeCommand);
    }

    protected static void UpdatePurpose(TestProcess testProcess, Purpose old, Purpose updated)
    {
        var updatePurposeCommand = $"{CommandNamer.PurposeName} {CommandNamer.Update} " +
                                   $"{OptionNamer.Name} {old.Key} " +
                                   $"{OptionNamer.Description} \"{updated.Description}\" " +
                                   $"{OptionNamer.NewName} {updated.Key}";

        testProcess.GiveInput(updatePurposeCommand);
    }

    protected static void ListPurpose(TestProcess testProcess)
    {
        testProcess.GiveInput($"{CommandNamer.PurposeName} {CommandNamer.List}");
    }

    protected static void DeletePurpose(TestProcess testProcess, Purpose purpose)
    {
        var deleteCommand = $"{CommandNamer.PurposeName} {CommandNamer.Delete} " +
                            $"{OptionNamer.Name} {purpose.Key}";
        testProcess.GiveInput(deleteCommand);
    }

    protected static void AddLegalBasis(TestProcess testProcess, LegalBasis legalBasis)
    {
        var addLegalBasisCommand = $"{CommandNamer.LegalBasisName} {CommandNamer.Create} " +
                                   $"{OptionNamer.Name} {legalBasis.Key} " +
                                   $"{OptionNamer.Description} \"{legalBasis.Description}\" ";
        testProcess.GiveInput(addLegalBasisCommand);
    }

    protected static void ShowLegalBasis(TestProcess testProcess, LegalBasis legalBasis)
    {
        var addLegalBasisCommand = $"{CommandNamer.LegalBasisName} {CommandNamer.Show} " +
                                   $"{OptionNamer.Name} {legalBasis.Key} ";
        testProcess.GiveInput(addLegalBasisCommand);
    }

    protected static void UpdateLegalBasis(TestProcess testProcess, LegalBasis old, LegalBasis updated)
    {
        var addLegalBasisCommand = $"{CommandNamer.LegalBasisName} {CommandNamer.Update} " +
                                   $"{OptionNamer.Name} {old.Key} " +
                                   $"{OptionNamer.Description} \"{updated.Description}\" " +
                                   $"{OptionNamer.NewName} {updated.Key}";
        testProcess.GiveInput(addLegalBasisCommand);
    }

    protected static void ListLegalBasis(TestProcess testProcess)
    {
        testProcess.GiveInput($"{CommandNamer.LegalBasisName} {CommandNamer.List}");
    }

    protected static void DeleteLegalBasis(TestProcess testProcess, LegalBasis legalBasis)
    {
        var addLegalBasisCommand = $"{CommandNamer.LegalBasisName} {CommandNamer.Delete} " +
                                   $"{OptionNamer.Name} {legalBasis.Key} ";
        testProcess.GiveInput(addLegalBasisCommand);
    }
    
    protected static void AddStoragePolicy(TestProcess testProcess, StoragePolicy storagePolicy)
    {
        var addDeleteConditionCommand = $"{CommandNamer.StoragePolicyName} {CommandNamer.Create} " +
                                        $"{OptionNamer.Name} {storagePolicy.Key} " +
                                        $"{OptionNamer.VacuumingCondition} \"{storagePolicy.VacuumingCondition}\" " +
                                        $"{OptionNamer.Description} \"{storagePolicy.Description}\"";

        testProcess.GiveInput(addDeleteConditionCommand);
    }

    protected static void ListStoragePolicies(TestProcess process)
    {
        process.GiveInput($"{CommandNamer.StoragePolicyAlias} {CommandNamer.List}");
    }

    protected static void UpdateStoragePolicy(TestProcess process, StoragePolicy old,
        StoragePolicy newStoragePolicy)
    {
        string command =
            $"{CommandNamer.StoragePolicyAlias} {CommandNamer.UpdateAlias} {OptionNamer.NameAlias} {old.Key}  {OptionNamer.NewNameAlias}  {newStoragePolicy.Key}" +
            $" {OptionNamer.VacuumingCondition} \"{newStoragePolicy.VacuumingCondition}\"" +
            $" {OptionNamer.Description} \"{newStoragePolicy.Description}\"";
        process.GiveInput(command);
    }

    protected static void UpdateStoragePolicyWithPersonalDataColumn(TestProcess process, StoragePolicy storagePolicy,
        PersonalDataColumn personalDataColumn)
    {
        var command =
            $"{CommandNamer.StoragePolicyAlias} {CommandNamer.UpdateAlias} {OptionNamer.NameAlias} {storagePolicy.Key} " +
            $" {OptionNamer.TableColumn} {personalDataColumn.Key.TableName} {personalDataColumn.Key.ColumnName}";
        process.GiveInput(command);
    }

    protected static void DeleteStoragePolicy(TestProcess process, StoragePolicy storagePolicy)
    {
        process.GiveInput($"{CommandNamer.StoragePolicyAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name}" +
                          $" {storagePolicy.Key}");
    }

    protected static void ShowStoragePolicy(TestProcess process, StoragePolicy storagePolicy)
    {
        process.GiveInput(
            $"{CommandNamer.StoragePolicyAlias} {CommandNamer.ShowAlias} -n {storagePolicy.Key}");
    }

    protected static void AddOrigin(TestProcess testProcess, Origin origin)
    {
        string command =
            $"{CommandNamer.OriginAlias} {CommandNamer.CreateAlias} {OptionNamer.Name} {origin.Key} {OptionNamer.Description} \"{origin.Description}\"";
        testProcess.GiveInput(command);
    }

    protected static void ListOrigins(TestProcess process)
    {
        process.GiveInput($"{CommandNamer.OriginAlias} {CommandNamer.List}");
    }

    protected static void UpdateOrigin(TestProcess process, Origin old, Origin newOrigin)
    {
        string command = $"{CommandNamer.OriginAlias} {CommandNamer.UpdateAlias} {OptionNamer.Name} " +
                         $"{old.Key} {OptionNamer.NewName} {newOrigin.Key} {OptionNamer.Description} \"{newOrigin.Description}\"";
        process.GiveInput(command);
    }

    protected static void DeleteOrigin(TestProcess process, Origin origin)
    {
        process.GiveInput(
            $"{CommandNamer.OriginAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name} {origin.Key}");
    }

    protected static void ShowOrigin(TestProcess process, Origin origin)
    {
        process.GiveInput(
            $"{CommandNamer.OriginAlias} {CommandNamer.ShowAlias} {OptionNamer.Name} {origin.Key}");
    }

    protected static void AddProcessing(TestProcess process, Processing processing)
    {
        string command =
            $"{CommandNamer.ProcessingName} {CommandNamer.Create} {OptionNamer.Name} {processing.Key}" +
            $" {OptionNamer.Description} {processing.Description}" +
            $" {OptionNamer.TableColumn} {processing.PersonalDataColumn.Key.TableName} {processing.PersonalDataColumn.Key.ColumnName}" +
            $" {OptionNamer.Purpose} {processing.Purpose.Key}";
        process.GiveInput(command);
    }

    protected static void ListProcessing(TestProcess process)
    {
        string command = $"{CommandNamer.ProcessingAlias} {CommandNamer.List}";
        process.GiveInput(command);
    }

    protected static void UpdateProcessing(TestProcess process, Processing old, Processing newProcess)
    {
        string command =
            $"{CommandNamer.ProcessingAlias} {CommandNamer.UpdateAlias} {OptionNamer.Name} {old.Key}" +
            $" {OptionNamer.NewNameAlias} {newProcess.Key}" +
            $" {OptionNamer.Description} {newProcess.Description}";
        process.GiveInput(command);
    }

    protected static void DeleteProcessing(TestProcess process, Processing processing)
    {
        string command =
            $"{CommandNamer.ProcessingAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name} {processing.Key}";
        process.GiveInput(command);
    }

    protected static void ShowProcessing(TestProcess process, Processing processing)
    {
        string command =
            $"{CommandNamer.ProcessingAlias} {CommandNamer.ShowAlias} {OptionNamer.Name} {processing.Key}";
        process.GiveInput(command);
    }

    protected static void AddPersonalDataColumn(TestProcess process, PersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnAlias} {CommandNamer.CreateAlias}" +
                      $" {OptionNamer.TableColumn} {personalDataColumn.Key.TableName} {personalDataColumn.Key.ColumnName}" +
                      $" {OptionNamer.DefaultValueAlias} \"{personalDataColumn.DefaultValue}\"" +
                      $" {OptionNamer.Description} \"{personalDataColumn.Description}\"" +
                      $" {OptionNamer.AssociationExpression} \"{personalDataColumn.AssociationExpression}\"" +
                      $" {OptionNamer.PurposeList} {string.Join(" ", personalDataColumn.Purposes.Select(p => p.Key))} " +
                      $" {OptionNamer.LegalBasisList} {string.Join(" ", personalDataColumn.LegalBases.Select(l => l.Key))} ";
        process.GiveInput(command);
    }

    protected static void UpdatePersonalDataColumn(TestProcess testProcess, PersonalDataColumn old,
        PersonalDataColumn updated)
    {
        var command = $"{CommandNamer.PersonalDataColumnAlias} {CommandNamer.Update} " +
                      $"{OptionNamer.TableColumn} {old.Key.TableName} {old.Key.ColumnName} " +
                      $"{OptionNamer.DefaultValue} \"{updated.DefaultValue}\" " +
                      $"{OptionNamer.AssociationExpression} \"{updated.AssociationExpression}\" " +
                      $"{OptionNamer.Description} \"{updated.Description}\" ";

        testProcess.GiveInput(command);
    }

    protected static void ShowPersonalDataColumn(TestProcess testProcess, PersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnAlias} {CommandNamer.Show} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} ";

        testProcess.GiveInput(command);
    }

    protected static void ListPersonalDataColumn(TestProcess testProcess)
    {
        var command = $"{CommandNamer.PersonalDataColumnAlias} {CommandNamer.List}";
        testProcess.GiveInput(command);
    }

    protected static void DeletePersonalDataColumn(TestProcess testProcess, PersonalDataColumn personalDataColumn)
    {
        var command = $"{CommandNamer.PersonalDataColumnAlias} {CommandNamer.Delete} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} ";
        testProcess.GiveInput(command);
    }

    protected static void AddPurposesToPersonalDataColumn(TestProcess testProcess, PersonalDataColumn personalDataColumn,
        IEnumerable<Purpose> purposes)
    {
        var command = $"{CommandNamer.PersonalDataColumnAlias} {CommandNamer.AddPurpose} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void RemovePurposesFromPersonalDataColumn(TestProcess testProcess,
        PersonalDataColumn personalDataColumn,
        IEnumerable<Purpose> purposes)
    {
        var command = $"{CommandNamer.PersonalDataColumnAlias} {CommandNamer.RemovePurpose} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void AddLegalBasesToPersonalDataColumn(TestProcess testProcess,
        PersonalDataColumn personalDataColumn, IEnumerable<LegalBasis> legalBases)
    {
        var command = $"{CommandNamer.PersonalDataColumnName} {CommandNamer.AddLegalBasis} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.LegalBasisList} {string.Join(" ", legalBases.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }
    
    protected static void RemoveLegalBasesFromPersonalDataColumn(TestProcess testProcess,
        PersonalDataColumn personalDataColumn, IEnumerable<LegalBasis> legalBases)
    {
        var command = $"{CommandNamer.PersonalDataColumnName} {CommandNamer.RemoveLegalBasis} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.LegalBasisList} {string.Join(" ", legalBases.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void CreatePersonalDataOrigin(TestProcess testProcess, int id,
        PersonalDataColumn personalDataColumn,
        Individual individual, Origin origin)
    {
        var command = $"{CommandNamer.PersonalDataOriginName} {CommandNamer.Create} " +
                      $"{OptionNamer.Id} {id} " +
                      $"{OptionNamer.TableColumn} {personalDataColumn.Key.TableName} " +
                      $"{personalDataColumn.Key.ColumnName} " +
                      $"{OptionNamer.Individual} {individual.Key} " +
                      $"{OptionNamer.Origin} {origin.Key}";

        testProcess.GiveInput(command);
    }

    protected static void SetIndividualsSource(TestProcess testProcess, TableColumnPair source)
    {
        var command = $"{CommandNamer.IndividualName} {CommandNamer.SetSource} " +
                      $"{OptionNamer.TableColumn} {source.TableName} {source.ColumnName}";
        testProcess.GiveInput(command);
    }

    protected static void ShowIndividualsSource(TestProcess testProcess)
    {
        var command = $"{CommandNamer.IndividualName} {CommandNamer.ShowSource}";
        testProcess.GiveInput(command);
    }

    protected static void ListIndividuals(TestProcess testProcess)
    {
        var command = $"{CommandNamer.IndividualName} {CommandNamer.List}";
        testProcess.GiveInput(command);
    }

    protected static void AddVacuumingPolicy(TestProcess testProcess, VacuumingPolicy vacuumingPolicy)
    {
        var command = $"{CommandNamer.VacuumingPolicyName} {CommandNamer.Create} " +
                      $"{OptionNamer.Name} {vacuumingPolicy.Key} " +
                      $"{OptionNamer.Duration} \"{vacuumingPolicy.Duration}\" " +
                      $"{OptionNamer.PurposeListAlias} {vacuumingPolicy.Purposes.First().ToListingIdentifier()} " +
                      $"{OptionNamer.Description} \"{vacuumingPolicy.Description}\"";
        testProcess.GiveInput(command);
    }

    protected static void UpdateVacuumingPolicy(TestProcess testProcess, VacuumingPolicy old, VacuumingPolicy updated)
    {
        var command = $"{CommandNamer.VacuumingPolicyName} {CommandNamer.Update} " +
                      $"{OptionNamer.Name} {old.Key} " +
                      $"{OptionNamer.NewName} {updated.Key} " +
                      $"{OptionNamer.Duration} \"{updated.Duration}\" " +
                      $"{OptionNamer.Description} \"{updated.Description}\"";
        testProcess.GiveInput(command);
    }

    protected static void ShowVacuumingPolicy(TestProcess testProcess, VacuumingPolicy vacuumingPolicy)
    {
        var command = $"{CommandNamer.VacuumingPolicyName} {CommandNamer.Show} " +
                      $"{OptionNamer.Name} {vacuumingPolicy.Key}";
        testProcess.GiveInput(command);
    }

    protected static void ListVacuumingPolicy(TestProcess testProcess)
    {
        var command = $"{CommandNamer.VacuumingPolicyName} {CommandNamer.List}";
        testProcess.GiveInput(command);
    }

    protected static void DeleteVacuumingPolicy(TestProcess testProcess, VacuumingPolicy vacuumingPolicy)
    {
        var command = $"{CommandNamer.VacuumingPolicyName} {CommandNamer.Delete} " +
                      $"{OptionNamer.Name} {vacuumingPolicy.Key}";
        testProcess.GiveInput(command);
    }

    protected static void ExecuteVacuumingPolicy(TestProcess testProcess,
        IEnumerable<VacuumingPolicy> vacuumingPolicies)
    {
        var command = $"{CommandNamer.VacuumingPolicyName} {CommandNamer.Execute} " +
                      $"{OptionNamer.VacuumingPolicyList} {string.Join(" ", vacuumingPolicies.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void AddPurposesToVacuumingPolicy(TestProcess testProcess, VacuumingPolicy vacuumingPolicy,
        IEnumerable<Purpose> purposes)
    {
        var command = $"{CommandNamer.VacuumingPolicyName} {CommandNamer.AddPurpose} " +
                      $"{OptionNamer.Name} {vacuumingPolicy.Key} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void RemovePurposesFromVacuumingPolicy(TestProcess testProcess, VacuumingPolicy vacuumingPolicy,
        IEnumerable<Purpose> purposes)
    {
        var command = $"{CommandNamer.VacuumingPolicyName} {CommandNamer.RemovePurpose} " +
                      $"{OptionNamer.Name} {vacuumingPolicy.Key} " +
                      $"{OptionNamer.PurposeList} {string.Join(" ", purposes.Select(p => p.Key))}";
        testProcess.GiveInput(command);
    }

    protected static void ReportStatus(TestProcess testProcess)
    {
        const string command = $"{CommandNamer.Status}";
        testProcess.GiveInput(command);
    }

    protected static void QuitProgram(TestProcess testProcess)
    {
        const string command = $"{CommandNamer.Quit}";
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
        var command = $"{CommandNamer.OriginName} {CommandNamer.Create} {OptionNamer.Name} {name}";
        testProcess.GiveInput(command);
    }

    protected static void CreateIndividual(TestProcess testProcess, Individual individual)
    {
        var command = $"{CommandNamer.IndividualName} {CommandNamer.Create} {OptionNamer.Id} {individual.Key}";
        testProcess.GiveInput(command);
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

    protected static void SetupTestData(IDbConnection dbConnection, TestProcess testProcess)
    {
        CreateIndividual(testProcess, TestIndividual1);
        CreateIndividual(testProcess, TestIndividual2);
        CreateIndividual(testProcess, TestIndividual3);

        CreatePersonalDataTable(dbConnection, TestPersonalDataColumn);

        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual1, $"{TestIndividual1.Id}");
        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual2, $"{TestIndividual2.Id}");
        InsertPersonalData(dbConnection, TestPersonalDataColumn, TestIndividual3, $"{TestIndividual3.Id}");
    }
}