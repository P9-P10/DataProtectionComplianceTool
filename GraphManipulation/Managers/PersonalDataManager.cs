﻿using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class PersonalDataManager : IPersonalDataManager
{
    private IMapper<PersonalDataColumn> _columnMapper;
    private IMapper<Purpose> _purposeMapper;
    private IMapper<Origin> _originMapper;
    private IMapper<PersonalData> _personDataMapper;
    private IMapper<Individual> _individualMapper;

    public PersonalDataManager(IMapper<PersonalDataColumn> columnMapper, IMapper<Purpose> purposeMapper, IMapper<Origin> originMapper, IMapper<PersonalData> personDataMapper, IMapper<Individual> individualMapper)
    {
        _columnMapper = columnMapper;
        _purposeMapper = purposeMapper;
        _originMapper = originMapper;
        _personDataMapper = personDataMapper;
        _individualMapper = individualMapper;
    }
    public IEnumerable<IPersonalDataColumn> GetAll()
    {
        return _columnMapper.Find(_ => true);
    }

    public IPersonalDataColumn? Get(TableColumnPair key)
    {
        return FindByKey(key);
    }

    public void Delete(TableColumnPair key)
    {
        _columnMapper.Delete(FindByKey(key));
    }

    public void UpdateDescription(TableColumnPair key, string description)
    {
        var column = FindByKey(key);
        column.Description = description;
        _columnMapper.Update(column);
    }

    public void AddPersonalData(TableColumnPair tableColumnPair, string joinCondition, string description)
    {
        _columnMapper.Insert(new PersonalDataColumn
            { TableColumnPair = tableColumnPair, Description = description, JoinCondition = joinCondition });
    }

    public void AddPurpose(TableColumnPair tableColumnPair, string purposeName)
    {
        var purpose = _purposeMapper.FindSingle(purpose => purpose.Name == purposeName);
        var column = FindByKey(tableColumnPair);
        column.Purposes = column.Purposes.Concat(new [] { purpose });
        _columnMapper.Update(column);
    }

    public void RemovePurpose(TableColumnPair tableColumnPair, string purposeName)
    {
        var purpose = _purposeMapper.FindSingle(purpose => purpose.Name == purposeName);
        var column = FindByKey(tableColumnPair);
        column.Purposes = column.Purposes.Where(p => !p.Equals(purpose));
        _columnMapper.Update(column);
    }

    public void SetOriginOf(TableColumnPair tableColumnPair, int individualsId, string originName)
    {
        var individual = _individualMapper.FindSingle(individual => individual.Id == individualsId);
        var origin = _originMapper.FindSingle(origin => origin.Name == originName);
        var column = FindByKey(tableColumnPair);
        var personData = new PersonalData() { Column = column, Origin = origin };
        individual.PersonalData = individual.PersonalData.Concat(new[] { personData });
        _personDataMapper.Insert(personData);
        _individualMapper.Update(individual);
    }

    public IOrigin GetOriginOf(TableColumnPair tableColumnPair, int individualsId)
    {
        var individual = _individualMapper.FindSingle(individual => individual.Id == individualsId);
        var personData = individual.PersonalData.FirstOrDefault(data => data.Column.TableColumnPair == tableColumnPair);
        return personData.Origin;
    }

    private PersonalDataColumn? FindByKey(TableColumnPair key)
    {
        return _columnMapper.FindSingle(column => column.TableColumnPair == key);
    }
}