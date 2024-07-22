function validateEmail(email) {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return re.test(String(email).toLowerCase());
}

document
  .getElementById("registerForm")
  .addEventListener("submit", function (event) {
    event.preventDefault();

    // Get form values
    const name = document.getElementById("name").value;
    const email = document.getElementById("email").value;
    const dateOfBirth = document.getElementById("dateOfBirth").value;
    const phone = document.getElementById("phone").value;
    const password = document.getElementById("password").value;
    const confirmPassword = document.getElementById("confirmPassword").value;

    // Validate inputs
    const errors = [];

    if (!name) {
      errors.push("Name is required.");
    }

    if (!email) {
      errors.push("Email is required.");
    } else if (!validateEmail(email)) {
      errors.push("Invalid email format.");
    }

    if (!dateOfBirth) {
      errors.push("Date of Birth is required.");
    } else if (new Date(dateOfBirth) >= new Date()) {
      errors.push("Date of Birth must be in the past.");
    }

    if (!phone) {
      errors.push("Phone number is required.");
    } else if (!/^\d{10}$/.test(phone)) {
      errors.push("Phone number must be 10 digits.");
    }

    if (!password) {
      errors.push("Password is required.");
    } else if (password.length < 6) {
      errors.push("Password must be at least 6 characters long.");
    }

    if (!confirmPassword) {
      errors.push("Confirm Password is required.");
    } else if (password !== confirmPassword) {
      errors.push("Password and Confirm Password do not match.");
    }

    if (errors.length > 0) {
      alert(errors.join("\n"));
      return;
    }

    // Format dateOfBirth to ISO 8601 format
    const formattedDateOfBirth = new Date(dateOfBirth).toISOString();

    // Send data to the endpoint
    const data = {
      name: name,
      email: email,
      dateOfBirth: formattedDateOfBirth,
      phone: phone,
      password: password,
      confirmPassword: confirmPassword,
    };

    fetch("http://localhost:5091/api/User/Register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    })
      .then((response) => response.json())
      .then((data) => {
        console.log("Success:", data);
        alert("Registration successful!");
      })
      .catch((error) => {
        console.error("Error:", error);
        alert("Registration failed.");
      });
  });
