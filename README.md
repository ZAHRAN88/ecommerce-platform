# E-Commerce Platform

A modern, feature-rich e-commerce platform built with ASP.NET Core MVC and Tailwind CSS. This application provides a seamless shopping experience with real-time cart management, wishlist functionality, and a responsive design.

## 🚀 Features

### Product Management
- Product catalog with category organization
- Detailed product views with specifications
- Quick view functionality
- Dynamic image handling
- Search and filtering capabilities

### Shopping Experience
- 🛒 Interactive shopping cart
  - Real-time updates
  - Session-based persistence
  - Quantity management
  - Dynamic cart counter
  - Form-based product additions

### Wishlist Management
- ❤️ Personal wishlist functionality
  - Add/Remove items
  - Move items to cart
  - Clear all functionality
  - Real-time updates
  - Session persistence

### User Interface
- 🎨 Modern, responsive design
- Tailwind CSS styling
- Interactive notifications (toast messages)
- Smooth animations and transitions
- Mobile-friendly layout
- Professional navigation and footer

## 🛠️ Technology Stack

- **Backend Framework**: ASP.NET Core MVC
- **Database**: Entity Framework Core
- **Frontend**: 
  - Tailwind CSS
  - JavaScript (Vanilla)
  - Font Awesome Icons
- **Development Tools**:
  - Visual Studio 2022
  - SQL Server
  - Git

## 📋 Prerequisites

- .NET 8.0 SDK 
- SQL Server (LocalDB or higher)
- Node.js (for npm packages)
- Visual Studio 2022 (recommended) or VS Code

## 🚀 Getting Started

1. **Clone the Repository**
   ```bash
   git clone https://github.com/ZAHRAN88/ecommerce-platform.git
   cd ecommerce-platform
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Set Up Database**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**
 


## 🏗️ Project Structure

```
Ecomm/
├── Controllers/         # MVC Controllers
├── Models/             # Data models and ViewModels
├── Views/              # Razor views
├── Data/              # DbContext and Migrations
├── wwwroot/           # Static files (CSS, JS, Images)
└── Program.cs         # Application startup
```

## 🔑 Key Features Implementation

### Shopping Cart
```csharp
// Example of AddToCart functionality
[HttpPost]
public async Task<IActionResult> AddToCart(int productId, int quantity)
{
    var sessionId = GetOrCreateSessionId();
    // Add item to cart logic
    return Json(new { success = true });
}
```

### Wishlist
```csharp
// Example of AddToWishlist functionality
[HttpPost]
public async Task<IActionResult> AddToWishlist(int productId)
{
    var sessionId = GetOrCreateSessionId();
    // Add item to wishlist logic
    return Json(new { success = true });
}
```

## 📱 Responsive Design

The application is fully responsive and provides an optimal viewing experience across a wide range of devices:
- 💻 Desktop
- 📱 Tablet
- 📱 Mobile

## 🔒 Security Features

- Session management
- CSRF protection
- XSS prevention
- Input validation
- Secure form handling

## 🎯 Future Enhancements

- [ ] User authentication and accounts
- [ ] Order processing system
- [ ] Payment gateway integration
- [ ] Admin dashboard
- [ ] Product reviews and ratings
- [ ] Email notifications
- [ ] Inventory management

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request





## 🙏 Acknowledgments

- ASP.NET Core team for the amazing framework
- Tailwind CSS team for the utility-first CSS framework
- Font Awesome for the beautiful icons
- All contributors who have helped with the project

## 📞 Contact


Project Link: [https://github.com/ZAHRAN88/ecommerce-platform](https://github.com/ZAHRAN88/ecommerce-platform) 