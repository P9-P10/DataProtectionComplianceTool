// using GraphManipulation.Logging;
// using GraphManipulation.Managers;
// using GraphManipulation.Managers.Archive;
// using GraphManipulation.Managers.Interfaces;
// using GraphManipulation.Managers.Interfaces.Archive;
// using GraphManipulation.Models.Interfaces;
//
// namespace GraphManipulation.Decorators.Managers;
//
// public class PersonalDataManagerDecorator : LoggingDecorator, IPersonalDataManager
// {
//
//     private readonly IPersonalDataManager _manager;
//     public PersonalDataManagerDecorator(IPersonalDataManager manager, ILogger logger) : base(logger, "personal data")
//     {
//         _manager = manager;
//     }
//     
//
//     public void SetOriginOf(TableColumnPair tableColumnPair, int individualsId, string originName)
//     {
//         LogUpdate(tableColumnPair.ToListing(), new {IndividualId = individualsId, Origin = originName});
//         _manager.SetOriginOf(tableColumnPair, individualsId, originName);
//     }
//
//     public IOrigin? GetOriginOf(TableColumnPair tableColumnPair, int individualsId)
//     {
//         return _manager.GetOriginOf(tableColumnPair, individualsId);
//     }
// }