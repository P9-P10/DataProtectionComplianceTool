// using GraphManipulation.DataAccess.Mappers;
// using GraphManipulation.Managers.Interfaces;
// using GraphManipulation.Managers.Interfaces.Archive;
// using GraphManipulation.Models;
// using GraphManipulation.Models.Interfaces;
//
// namespace GraphManipulation.Managers.Archive;
//
// public class IndividualsManager : IIndividualsManager
// {
//     private readonly IMapper<Individual> _mapper;
//     private readonly IMapper<ConfigKeyValue> _keyValueMapper;
//     private const string IndividualsSourceKey = "IndividualsSource";
//
//     public IndividualsManager(IMapper<Individual> individualsMapper, IMapper<ConfigKeyValue> keyValueMapper)
//     {
//         _mapper = individualsMapper;
//         _keyValueMapper = keyValueMapper;
//     }
//
//     public void SetIndividualsSource(TableColumnPair source)
//     {
//         var keyValue = _keyValueMapper.FindSingle(x => x.Key == IndividualsSourceKey);
//         if (keyValue == null)
//         {
//             _keyValueMapper.Insert(new ConfigKeyValue
//                 { Key = IndividualsSourceKey, Value = $"({source.TableName},{source.ColumnName})" });
//         }
//         else
//         {
//             keyValue.Value = $"({source.TableName},{source.ColumnName})";
//             _keyValueMapper.Update(keyValue);
//         }
//     }
//
//     public TableColumnPair GetIndividualsSource()
//     {
//         var keyValue = _keyValueMapper.FindSingle(x => x.Key == IndividualsSourceKey);
//         var table = keyValue.Value.Split(",")[0].Replace("(", "");
//         var column = keyValue.Value.Split(",")[1].Replace(")", "");
//         var tableColumnPair = new TableColumnPair(table, column);
//         return tableColumnPair;
//     }
// }