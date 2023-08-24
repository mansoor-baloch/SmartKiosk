# SmartKiosk
The advancements in Human-Computer Interaction and IoT-based technologies make it imperative to build and deploy user friendly applications
to facilitate people and contribute largely to the automated world. Self-ordering kiosk, as the name indicates, is developed to enable customers
at restaurants and remote spaces to place their orders in an effecient and convenient manner. Both cash and cashless payment solutions are 
integrated with the kiosk, providing a real-time expereince to the users and the restaurant staff. The live monitoring of the sales and inventory 
data and reports generation through a web portal adds to the top of resource management. The application is designed in such a way that the user 
spends least possible time for placing a single order while making it a near-to-perfect solution for places having big queues. Being in the production 
environment, the machine is still under the user-testing phase of deplyoment. Restaurants anywhere can customize the standalone machine according 
to their specifications. 

# Self-Ordering Kiosk Software

![Project Logo](/path/to/logo.png) <!-- Replace with the path to your project logo -->

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

## Prerequisites

- [.NET Core SDK X.X](https://dotnet.microsoft.com/download) or later
- [SQL Server X.X](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or later
- Web browser for accessing the dashboard

## Installation

1. Clone this repository to your local machine:
- git clone https://github.com/your-username/self-ordering-kiosk.git


2. Navigate to the project directory:
- cd self-ordering-kiosk


3. Build the solution:
- dotnet build


## Configuration

1. Set up the SQL Server database by executing the SQL scripts located in the `database-scripts` folder.

2. Configure the project settings in the `appsettings.json` file, including:
- Database connection strings
- Payment gateway credentials

## Usage

1. Run the application:
- dotnet run


2. Access the kiosk interface by opening a web browser and navigating to `http://localhost:5000`.

3. Test the different functionalities of the kiosk software, including order placement and payment.

4. For the web dashboard, access it by navigating to `http://localhost:5000/dashboard` in your web browser. Log in to view order summaries and sales data.

## Contributing

We welcome contributions to improve the Self-Ordering Kiosk Software! To contribute:

1. Fork the repository.

2. Create a new branch for your feature or bug fix.

3. Make your changes and commit them with descriptive messages.

4. Push your changes to your forked repository.

5. Create a pull request to this repository's `main` branch.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For any inquiries or feedback, please reach out to us at your-email@example.com.

---

[Optional: Include badges for build status, license, stars, etc.]


