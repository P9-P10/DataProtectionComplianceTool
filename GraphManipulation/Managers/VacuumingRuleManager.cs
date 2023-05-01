using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Managers;

public class VacuumingRuleManager : NamedEntityManager<VacuumingRule>, IVacuumingRulesManager
{
    private readonly IMapper<VacuumingRule> _ruleMapper;
    private readonly IMapper<Purpose> _purposeMapper;
    private readonly IVacuumer _vacuumer;
    

    public VacuumingRuleManager(IMapper<VacuumingRule> ruleMapper,IMapper<Purpose> purposeMapper, IVacuumer vacuumer) : base(ruleMapper)
    {
        _ruleMapper = ruleMapper;
        _purposeMapper = purposeMapper;
        _vacuumer = vacuumer;
    }

    public IEnumerable<IVacuumingRule> GetAll() => base.GetAll();
    public IVacuumingRule? Get(string key) => base.Get(key);

    public void AddVacuumingRule(string name, string interval, string purposeName)
    {
        _ruleMapper.Insert(new VacuumingRule(name:name, interval:interval, 
            purposes:new List<Purpose> {_purposeMapper.FindSingle(x=> x.Name == purposeName)}
            )
        );
    }

    public void UpdateInterval(string name, string interval)
    {
        VacuumingRule? rule = base.Get(name);
        if (rule == null) return;
        rule.Interval = interval;
        _ruleMapper.Update(rule);
    }

    public void ExecuteRule(string name)
    {
        var rule = base.Get(name);
        if (rule is null) return;

        _vacuumer.ExecuteVacuumingRules(new[] { rule });
    }

    public void AddPurpose(string name, string purposeName)
    {
        VacuumingRule? rule = base.Get(name);
        if (rule == null) return;
        rule.Purposes.Append(_purposeMapper.FindSingle(x => x.Name == purposeName));
        _ruleMapper.Update(rule);
    }

    public void RemovePurpose(string name, string purposeName)
    {
        var purpose = _purposeMapper.FindSingle(purpose => purpose.Name == purposeName);
        VacuumingRule rule = base.Get(name);
        rule.Purposes =rule.Purposes.Where(p => !p.Equals(purpose));
        _ruleMapper.Update(rule);
    }

    public void UpdateDescription(string key, string description)
    {
        VacuumingRule? rule = base.Get(key);
        if (rule == null) return;
        rule.Description = description;
        _ruleMapper.Update(rule);
    }
}