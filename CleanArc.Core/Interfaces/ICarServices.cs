using CleanArc.Core.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Core.Interfaces
{
    public interface ICarServices
    {
        Task<Car> Create(Car car);
        Task<IEnumerable<Car>> GetAllCars();
        Task<Car> GetCarById(int id);
        void UpdateCar(Car car);
        
    }
}
