using DoeTech.Data;
using DoeTech.DTOs;
using DoeTech.Exceptions;
using DoeTech.Models;

namespace DoeTech.Repositories;

public class EquipmentRepository : IRepository
{
    private readonly AppDbContext _dbContext;

    public EquipmentRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    public void Add(Equipment equipment)
    {
        
        _dbContext.Equipments.Add(equipment);
        _dbContext.SaveChanges();
    }

    public List<Equipment> GetList()
    {
        return _dbContext.Equipments
            .Where(e => e.Removed == false)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
        ;
    }

    public void Update(Equipment equipment)
    {
        equipment.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _dbContext.Equipments.Update(equipment);
        _dbContext.SaveChanges();
    }
    
    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    public void RemoveById(Guid id)
    {
        var equipmentToRemove = _dbContext.Equipments.Find(id);

        if (equipmentToRemove == null)
        {
            throw new NotFoundException("equipment not found to remove: " + id);
        }

        equipmentToRemove.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        equipmentToRemove.Removed = true;
        _dbContext.Equipments.Update(equipmentToRemove);
        _dbContext.SaveChanges();
    }

    public Equipment GetById(Guid id)
    {
        return _dbContext.Equipments.FirstOrDefault(e => e.Id == id && !e.Removed);
    }
    
    public List<Equipment> Query(EquipmentQueryDto filter)
    { 
        var query = _dbContext.Equipments.AsQueryable();

        if (filter.AccountId != null) {
            query = query.Where(e => e.DonorAccountId == filter.AccountId);
        }
        
        if (filter.CreatedAtFrom != null)
        {
            query = query.Where(a => a.CreatedAt <= filter.CreatedAtFrom);
        }

        if (filter.CreatedAtTo != null)
        {
            query = query.Where(a => a.CreatedAt <= filter.CreatedAtTo);
        }

        query = query.Where(e => !e.Removed)
            .OrderBy(e => e.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return query.ToList();
    }
    
    public List<Equipment> GetByIds(IEnumerable<Guid> equipmentIds)
    {
        return _dbContext.Equipments
            .Where(e => equipmentIds.Contains(e.Id))
            .ToList();
    }
}