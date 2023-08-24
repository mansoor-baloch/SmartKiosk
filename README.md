# Self-Ordering Kiosk Software

Welcome to the Self-Ordering Kiosk Software project! This software application provides a self-service solution for ordering and paying for items through a kiosk. The software integrates with various payment methods, cash acceptor devices, and POS machines to offer a seamless experience for customers.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features

- **Order Placement:** Customers can browse and select items for purchase using an intuitive user interface.
- **Payment Options:** Supports multiple payment methods, including cash, QR code-based payments, and integration with Bank Alfalah, Easypaisa, and Jazzcash.
- **Real-time Data:** Utilizes IoT technology to send and receive data in real-time to and from the server, providing up-to-date order information.
- **Dashboard:** A web-based dashboard visualizes order summaries and sales data for easy monitoring and analysis.
- **POS Integration:** Seamlessly interacts with POS machines for unattended order placement and payment.
- **Optimizing Queues:** Makes the user spend least possible time for placing a single order while making it a near-to-perfect solution for places having big queues.

## Prerequisites

- [.NET Core](https://dotnet.microsoft.com/en-us/download) or later
- [SQL Server 2019](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or later
- Web browser for accessing the dashboard

## Installation

1. Clone this repository to your local machine:
	```bash
 	git clone https://github.com/your-username/self-ordering-kiosk.git


2. Navigate to the project directory:
	 cd self-ordering-kiosk


3. Build the solution:
- dotnet build


## Configuration

1. Sets up the SQL Server database by executing the SQL scripts and creates a relational database.

2. Configure the project settings in the `App.config` file, including:
- Database connection strings
- Payment gateway credentials

## Usage

1. Run the application:
- dotnet run exe

2. Access the kiosk interface by opening a web browser and navigating to `https://webportal.vendingc.com/`.

3. Test the different functionalities of the kiosk software, including order placement and payment.

4. For the web dashboard, access it by navigating to `https://webportal.vendingc.com/` in your web browser. Log in to view order summaries and sales data.

## Contributing

We welcome contributions to improve the Self-Ordering Kiosk Software! To contribute:

1. Fork the repository.

2. Create a new branch for your feature or bug fix.

3. Make your changes and commit them with descriptive messages.

4. Push your changes to your forked repository.

5. Create a pull request to this repository's `main` branch.

## License

This project is licensed under the [VendingC License](https://www.vendingc.com/).

## Contact

For any inquiries or feedback, please reach out to us at mansoor.ahmed@lums.edu.pk

---

