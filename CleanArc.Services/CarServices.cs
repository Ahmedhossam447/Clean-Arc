using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Infrastructure.Persistence
{
    public class CarServices : ICarServices
    {
        private readonly IRepository<Car> _carRepository;
        public CarServices(IRepository<Car> carRepository)
        {
            _carRepository = carRepository;
        }
        public async Task<Car> Create(Car car)
        {
           return await _carRepository.Add(car);
        }

        public Task<IEnumerable<Car>> GetAllCars()
        {
            return _carRepository.GetAll();
        }

        public Task<Car> GetCarById(int id)
        {
            return _carRepository.GetById(id);
        }

        public void UpdateCar(Car car)
        {
            _carRepository.Update(car);
        }
    }
}
