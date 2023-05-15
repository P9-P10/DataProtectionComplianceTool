// using GraphManipulation.DataAccess.Mappers;
// using GraphManipulation.Managers.Interfaces;
// using GraphManipulation.Managers.Interfaces.Archive;
// using GraphManipulation.Models;
// using GraphManipulation.Models.Interfaces;
//
// namespace GraphManipulation.Managers.Archive;
//
// public class PersonalDataManager : IPersonalDataManager
// {
//     private IMapper<PersonalDataColumn> _columnMapper;
//     private IMapper<Purpose> _purposeMapper;
//     private IMapper<Origin> _originMapper;
//     private IMapper<PersonalDataOrigin> _personalDataMapper;
//     private IMapper<Individual> _individualMapper;
//
//     public PersonalDataManager(IMapper<PersonalDataColumn> columnMapper, IMapper<Purpose> purposeMapper,
//         IMapper<Origin> originMapper, IMapper<PersonalDataOrigin> personalDataMapper, IMapper<Individual> individualMapper)
//     {
//         _columnMapper = columnMapper;
//         _purposeMapper = purposeMapper;
//         _originMapper = originMapper;
//         _personalDataMapper = personalDataMapper;
//         _individualMapper = individualMapper;
//     }

//     public void SetOriginOf(TableColumnPair tableColumnPair, int individualsId, string originName)
//     {
//         var individual = _individualMapper.FindSingle(individual => individual.Id == individualsId)!;
//         var origin = _originMapper.FindSingle(origin => origin.Name == originName)!;
//         var column = FindByKey(tableColumnPair)!;
//         
//         var personalData = new PersonalDataOrigin { PersonalDataColumn = column, Origin = origin };
//         
//         if (individual.PersonalDataOrigin is null)
//         {
//             individual.PersonalDataOrigin = new List<PersonalDataOrigin> { personalData };
//         }
//         else
//         {
//             var personalDataList = individual.PersonalDataOrigin.ToList();
//             personalDataList.Add(personalData);
//             individual.PersonalDataOrigin = personalDataList;
//         }
//
//         _personalDataMapper.Insert(personalData);
//         _individualMapper.Update(individual);
//     }
//
//     public IOrigin? GetOriginOf(TableColumnPair tableColumnPair, int individualsId)
//     {
//         var individual = _individualMapper.FindSingle(individual => individual.Id == individualsId);
//         var personalData =
//             individual?.PersonalDataOrigin?.FirstOrDefault(data =>
//                 data.PersonalDataColumn.TableColumnPair.Equals(tableColumnPair));
//         return personalData?.Origin;
//     }
//
//     private PersonalDataColumn? FindByKey(TableColumnPair key)
//     {
//         return _columnMapper.FindSingle(column => column.TableColumnPair.Equals(key));
//     }
// }