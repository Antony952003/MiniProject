use dbMovieBooking
use dbRequestTracker14May24

select * from Employees
use master
select * from Showtimes
select * from Theaters
select * from Movies
select * from castmembers
select * from Seats
select * from Screens
select * from ShowtimeSeats
select * from Showtimes
select * from snacks
select * from Bookings
select * from Payments
select * from userpoints
select * from Artists
select * from castmembers
select * from BookingSnacks
select * from cancellations
select * from reviews
select * from seats
select * from Users
select * from UserDetails
select * from movies

UPDATE Users
SET Image = 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ4NbI143XzQJBya031NMvUqkfHS52LTnRqmQ&s'
WHERE id= 5;

select * from movies

update showtimes set starttime = '2024-07-10 18:00:00.0000000'
where starttime = '2024-07-09 18:00:00.0000000'


update movies
set bannerimage = 'https://images7.alphacoders.com/107/1073088.jpg'
where movieid = 5

select * from artists

update artists set UserImage='https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT09pKSzw78IuESAqHApXqb7Afcl45EZPr9TFrRGDpMT-TNmT99'
where artistid = 23

update movies
set 
mainimage = 'https://image.tmdb.org/t/p/original/lkZ9gqCEjzX85lKR6Jjd1uGAXNp.jpg'
where movieid = 5

sp_help UserDetails
