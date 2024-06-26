﻿using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Contexts;
using MovieBookingAPI.Exceptions;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Repositories
{
    public class ShowtimeSeatRepository : IRepository<int, ShowtimeSeat>
    {
        private readonly MovieBookingContext _context;
        public ShowtimeSeatRepository(MovieBookingContext context)
        {
            _context = context;
        }

        public async Task<ShowtimeSeat> Add(ShowtimeSeat item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<ShowtimeSeat> Delete(int key)
        {
            var showtimeseat = await Get(key);
            if (showtimeseat != null)
            {
                _context.Remove(showtimeseat);
                await _context.SaveChangesAsync();
                return showtimeseat;
            }
            throw new EntityNotFoundException("ShowtimeSeat");
        }

        public async Task<ShowtimeSeat> Get(int key)
        {
            var showtimeseats = await Get();
            if (showtimeseats == null) return null;
            var showtimeseat = showtimeseats.ToList().Find(x => x.ShowtimeSeatId == key);
            if (showtimeseat != null)
                return showtimeseat;
            return null;
        }

        public async Task<IEnumerable<ShowtimeSeat>> Get()
        {
            var result = await _context.ShowtimeSeats
                .Include(x => x.Showtime)
                .Include(x => x.Seat)
                .ToListAsync();
            if (result.Count == 0)
                return null;
            return result;
        }

        public async Task<ShowtimeSeat> Update(ShowtimeSeat item)
        {
            var showtimeseat = await Get(item.ShowtimeSeatId);
            if (showtimeseat != null)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return showtimeseat;
            }
            throw new EntityNotFoundException("ShowtimeSeat");
        }
    }
}
