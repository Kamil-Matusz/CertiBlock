import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import AdminApp from "./admin/AdminApp";

function App() {
    return (
        <Router>
            <div style={{ padding: 20 }}>
                <h1>CertiBlock Frontend</h1>
                <nav>
                    <Link to="/">Home</Link> | <Link to="/admin">Admin Panel</Link>
                </nav>
            </div>

            <Routes>
                <Route path="/" element={<div>Welcome to CertiBlock</div>} />
                <Route path="/admin/*" element={<AdminApp />} />
            </Routes>
        </Router>
    );
}

export default App;
