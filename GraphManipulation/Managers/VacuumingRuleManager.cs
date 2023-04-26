using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class VacuumingRuleManager : NamedEntityManager<VacuumingRule>, IVacuumingRulesManager
{
    private IMapper<VacuumingRule> _vacuumingRule;
    private IMapper<Purpose> _purposeMapper;

    public VacuumingRuleManager(IMapper<VacuumingRule> vacuumingRule,IMapper<Purpose> purposeMapper) : base(vacuumingRule)
    {
        _vacuumingRule = vacuumingRule;
        _purposeMapper = purposeMapper;
    }

    public IEnumerable<IVacuumingRule> GetAll() => base.GetAll();
    public IVacuumingRule? Get(string key) => base.Get(key);

    public void AddVacuumingRule(string name, string interval, string purposeName)
    {
        _vacuumingRule.Insert(new VacuumingRule(name:name, interval:interval, 
            purposes:new List<Purpose> {_purposeMapper.FindSingle(x=> x.Name == purposeName)}
            )
        );
    }

    public void UpdateInterval(string name, string interval)
    {
        VacuumingRule? rule = base.Get(name);
        if (rule == null) return;
        rule.Interval = interval;
        _vacuumingRule.Update(rule);
    }

    public void UpdateDescription(string key, string description)
    {
        VacuumingRule? rule = base.Get(key);
        if (rule == null) return;
        rule.Description = description;
        _vacuumingRule.Update(rule);
    }
}