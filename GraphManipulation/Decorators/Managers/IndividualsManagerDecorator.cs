// using GraphManipulation.Logging;
// using GraphManipulation.Managers;
// using GraphManipulation.Managers.Archive;
// using GraphManipulation.Managers.Interfaces;
// using GraphManipulation.Managers.Interfaces.Archive;
// using GraphManipulation.Models.Interfaces;
//
// namespace GraphManipulation.Decorators.Managers;
//
// public class IndividualsManagerDecorator : LoggingDecorator, IIndividualsManager
// {
//     private IIndividualsManager _manager;
//
//     public IndividualsManagerDecorator(IIndividualsManager manager, ILogger logger) :
//         base(logger, "individual")
//     {
//         _manager = manager;
//     }
//
//     public void SetIndividualsSource(TableColumnPair source)
//     {
//         LogSet(source.ToListing(),source);
//         _manager.SetIndividualsSource(source);
//     }
//
//     public TableColumnPair GetIndividualsSource()
//     {
//         return _manager.GetIndividualsSource();
//     }
// }