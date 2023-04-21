using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class VacuumingRuleManager : NamedEntityManager<VacuumingRule>, IVacuumingRulesManager
{
    private IMapper<VacuumingRule> _mapper;

    public VacuumingRuleManager(IMapper<VacuumingRule> mapper) : base(mapper)
    {
        _mapper = mapper;
    }

    public IEnumerable<IVacuumingRule> GetAll() => base.GetAll();


    public IVacuumingRule? Get(string key) => base.Get(key);

    public void AddVacuumingRule(string name, string interval, string purposeName)
    {
        _mapper.Insert(new VacuumingRule(name:name, interval:interval, 
            purposes:new List<Purpose>() {new() {Name = purposeName}}
            )
        );
    }

    public void UpdateInterval(string name, string interval)
    {
        VacuumingRule? rule = base.Get(name);
        if (rule == null) return;
        rule.Interval = interval;
        _mapper.Update(rule);
    }

    public void UpdateDescription(string key, string description)
    {
        VacuumingRule? rule = base.Get(description);
        if (rule == null) return;
        rule.Description = description;
        _mapper.Update(rule);
        
    }
}