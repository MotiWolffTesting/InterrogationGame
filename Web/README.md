# InterrogationGame - Web UI Version

## 🌐 Enhanced Web Interface

This is the enhanced web UI version of the Interrogation Game, providing a modern, interactive interface with drag-and-drop sensor assignment and real-time feedback.

## ✨ Features

### 🎮 Modern Game Interface
- **Drag-and-Drop Sensor Assignment**: Intuitive sensor placement
- **Real-time Progress Tracking**: Visual progress bars and status updates
- **Responsive Design**: Works on desktop, tablet, and mobile devices
- **Interactive Feedback**: Live sensor status and special ability feedback

### 🔧 Technical Enhancements
- **RESTful API**: Clean separation between frontend and backend
- **Swagger Documentation**: Auto-generated API documentation
- **CORS Support**: Cross-origin resource sharing enabled
- **Async Operations**: Non-blocking game operations
- **Error Handling**: Comprehensive error management

### 🎯 Game Mechanics
- **Visual Sensor Slots**: Clear indication of available and occupied slots
- **Sensor Palette**: Easy access to all available sensor types
- **Progress Visualization**: Real-time progress tracking with visual indicators
- **Victory Modals**: Celebratory victory screens
- **Auto-refresh**: Automatic status updates every 5 seconds

## 🚀 Getting Started

### Running the Web Version

1. **Start the web server**:
   ```bash
   dotnet run -- --web
   ```

2. **Access the web interface**:
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`
   - The web interface will load automatically

3. **API Documentation**:
   - Visit `https://localhost:5001/swagger` for interactive API documentation
   - Test API endpoints directly from the browser

### Running the Console Version

For the traditional console experience:
```bash
dotnet run
```

## 🎮 How to Play (Web Version)

### 1. Starting a New Game
- Click the **"Start New Game"** button
- A new terrorist target will be assigned
- View target information in the info cards

### 2. Assigning Sensors
- **Drag sensors** from the sensor palette to empty slots
- **Visual feedback** shows which slots are occupied
- **Assign button** becomes active when sensors are placed

### 3. Activating Sensors
- Click **"Assign Sensors"** to activate all placed sensors
- View **real-time feedback** in the feedback panel
- **Progress bar** updates to show current status

### 4. Managing Turns
- Click **"End Turn"** to progress to the next turn
- **Counterattacks** may occur based on terrorist rank
- **Sensor status** updates automatically

### 5. Victory Conditions
- **Victory modal** appears when all required sensors match
- **Automatic progression** to next terrorist if available
- **Mission complete** when all terrorists are exposed

## 🔌 API Endpoints

### Game Management
- `POST /api/WebGame/start` - Start a new game
- `GET /api/WebGame/status` - Get current game status
- `POST /api/WebGame/assign-sensors` - Assign sensors to slots
- `POST /api/WebGame/end-turn` - End current turn
- `POST /api/WebGame/victory` - Mark current terrorist as exposed

### Information
- `GET /api/WebGame/available-sensors` - Get list of available sensors

## 🎨 UI Features

### Visual Design
- **Modern Glassmorphism**: Translucent panels with backdrop blur
- **Gradient Backgrounds**: Professional military-style gradients
- **Smooth Animations**: Hover effects and transitions
- **Responsive Grid**: Adapts to different screen sizes

### Interactive Elements
- **Drag-and-Drop**: Intuitive sensor assignment
- **Hover Effects**: Visual feedback on interactive elements
- **Loading States**: Spinner animations during operations
- **Modal Dialogs**: Victory celebrations and confirmations

### Real-time Updates
- **Auto-refresh**: Status updates every 5 seconds
- **Live Progress**: Real-time progress bar updates
- **Dynamic Feedback**: Sensor status changes immediately
- **Error Handling**: User-friendly error messages

## 🔧 Technical Architecture

### Frontend
- **Vanilla JavaScript**: No framework dependencies
- **CSS Grid & Flexbox**: Modern layout techniques
- **Fetch API**: Modern HTTP requests
- **Drag-and-Drop API**: Native browser functionality

### Backend
- **ASP.NET Core Web API**: RESTful service layer
- **Dependency Injection**: Clean service management
- **Entity Framework Core**: Database operations
- **Swagger/OpenAPI**: Auto-generated documentation

### Data Flow
1. **User Interaction** → JavaScript event handlers
2. **API Requests** → RESTful endpoints
3. **Game Logic** → Shared GameManager service
4. **Database Operations** → Entity Framework
5. **Response** → JSON data back to frontend
6. **UI Updates** → Dynamic DOM manipulation

## 🎯 Advantages Over Console Version

### User Experience
- **Visual Feedback**: See sensor status at a glance
- **Drag-and-Drop**: Intuitive sensor assignment
- **Real-time Updates**: Live progress tracking
- **Responsive Design**: Works on any device

### Development Benefits
- **API-First Design**: Clean separation of concerns
- **Documentation**: Auto-generated API docs
- **Testing**: Easy to test individual endpoints
- **Extensibility**: Easy to add new features

### Accessibility
- **Web Standards**: Follows modern web practices
- **Cross-Platform**: Works on any modern browser
- **No Installation**: Runs directly in the browser
- **Shareable**: Can be hosted and shared easily

## 🔮 Future Enhancements

### Planned Features
- **Sound Effects**: Audio feedback for actions
- **Dark/Light Mode**: Theme switching
- **Multiplayer Support**: Collaborative interrogation
- **Advanced Analytics**: Detailed performance tracking
- **Mobile App**: Native mobile application

### Technical Improvements
- **WebSocket Support**: Real-time bidirectional communication
- **Progressive Web App**: Offline capability
- **Service Worker**: Background sync
- **Push Notifications**: Turn reminders

## 🐛 Troubleshooting

### Common Issues
1. **Port Conflicts**: Ensure ports 5000/5001 are available
2. **Database Connection**: Check PostgreSQL connection string
3. **CORS Issues**: Verify CORS policy configuration
4. **Browser Compatibility**: Use modern browsers (Chrome, Firefox, Safari, Edge)

### Debug Mode
- **Swagger UI**: Visit `/swagger` for API testing
- **Browser Console**: Check for JavaScript errors
- **Network Tab**: Monitor API requests
- **Application Logs**: Check server-side logs

## 📝 License

This web version is part of the InterrogationGame project for educational purposes as part of IDF 8200 training exercises. 