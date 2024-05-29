# Movie Booking API

Welcome to the documentation for the Movie Booking API. This API allows users to manage movie bookings, theaters, screens, snacks, and more.

## Base URL

The base URL for all API endpoints is `http://localhost:5091/api/`.

## Endpoints

### User

- **Login**
  - URL: `/User/Login`
  - Description: Endpoint for user login.

- **Register**
  - URL: `/User/Register`
  - Description: Endpoint for user registration.

### Theater

- **Add Theater**
  - URL: `/Theater/AddTheater`
  - Description: Endpoint to add a new theater.

- **Get Theater by ID**
  - URL: `/Theater/GetTheaterById?theaterid={theaterid}`
  - Description: Endpoint to retrieve theater details by ID.

- **Get Theater by Name**
  - URL: `/Theater/GetTheaterByName?theaterName={theaterName}`
  - Description: Endpoint to retrieve theater details by name.

- **Update Theater Location**
  - URL: `/Theater/UpdateTheaterLocation?theaterid={theaterid}&location={location}`
  - Description: Endpoint to update the location of a theater.

### Screen

- **Add Screen to Theater**
  - URL: `/Screen/AddScreenToTheater`
  - Description: Endpoint to add a new screen to a theater.

- **Get Screen by ID**
  - URL: `/Screen/GetScreenById?screenid={screenid}`
  - Description: Endpoint to retrieve screen details by ID.

- **Get Screen by Screen Name**
  - URL: `/Screen/GetScreenByScreenName?screenName={screenName}`
  - Description: Endpoint to retrieve screen details by name.

### ShowtimeSeat

- **Get Available Showtime Seats**
  - URL: `/ShowtimeSeat/GetAvailableShowtimeSeats?showtimeid={showtimeid}`
  - Description: Endpoint to retrieve available showtime seats by showtime ID.

- **Add Showtime Seats**
  - URL: `/ShowtimeSeat/AddShowtimeSeats`
  - Description: Endpoint to add new showtime seats.

- **Get Showtime Seats Available in Range**
  - URL: `/ShowtimeSeat/GetShowtimeSeatsAvailableInRange`
  - Description: Endpoint to retrieve showtime seats available in a specified range.

- **Get Consecutively Available Showtime Seats in Range**
  - URL: `/ShowtimeSeat/GetShowtimeSeatsConsecutivelyAvailableInRange`
  - Description: Endpoint to retrieve consecutively available showtime seats in a specified range.

### Booking

- **Add Booking**
  - URL: `/Booking/AddBooking`
  - Description: Endpoint to add a new booking.

- **Get All Bookings**
  - URL: `/Booking/GetAllBookings`
  - Description: Endpoint to retrieve all bookings.

- **Get All User Bookings**
  - URL: `/Booking/GetAllUserBookings`
  - Description: Endpoint to retrieve all bookings for a specific user.

### Cancellation

- **Process Cancellation**
  - URL: `/Cancellation/ProcessCancellation`
  - Description: Endpoint to process booking cancellation.

### Payment

- **Process Payment**
  - URL: `/Payment/ProcessPayment`
  - Description: Endpoint to process payment for a booking.

### Review

- **Give Review**
  - URL: `/Review/GiveReview`
  - Description: Endpoint to submit a review for a movie.

- **Get Movie Reviews**
  - URL: `/Review/GetMovieReviews?moviename={moviename}`
  - Description: Endpoint to retrieve reviews for a specific movie.

### Seat

- **Generate Seats**
  - URL: `/Seat/GenerateSeats`
  - Description: Endpoint to generate seats for a theater screen.

### Movie

- **Add Movie**
  - URL: `/Movie/AddMovie`
  - Description: Endpoint to add a new movie.

- **Get Most Reviewed Movies**
  - URL: `/Movie/GetMostReviewedMovies`
  - Description: Endpoint to retrieve the most reviewed movies.

