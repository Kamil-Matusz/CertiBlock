import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import AdminApp from "./admin/AdminApp";
import { RegisterPage } from "./admin/components/Auth/RegisterPage";
import { CertificateCreateForm } from "./components/Certificates/CertificateCreateForm";
import { useEffect, useState } from "react";

function App() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);

    useEffect(() => {
        const checkAuth = () => {
            const token = localStorage.getItem('token');
            setIsLoggedIn(!!token);
        };

        checkAuth();
        const interval = setInterval(checkAuth, 1000);
        return () => clearInterval(interval);
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('userRole');
        setIsLoggedIn(false);
        window.location.href = '/';
    };

    return (
        <Router>
            <div style={{ padding: 20 }}>
                <h1>CertiBlock Frontend</h1>
                <nav>
                    <Link to="/">Home</Link>
                    {' | '}

                    {!isLoggedIn && (
                        <>
                            <Link to="/admin">Login</Link>
                            {' | '}
                            <Link to="/register">Register</Link>
                        </>
                    )}

                    {isLoggedIn && (
                        <>
                            <Link to="/admin">Admin Panel</Link>
                            {' | '}
                            <Link to="/certificates/create">Create Certificate</Link>
                            {' | '}
                            <button
                                onClick={handleLogout}
                                style={{
                                    cursor: 'pointer',
                                    background: 'none',
                                    border: 'none',
                                    color: 'blue',
                                    fontSize: 'inherit',
                                    padding: 0
                                }}
                            >
                                Logout
                            </button>
                        </>
                    )}
                </nav>
            </div>

            <Routes>
                <Route path="/" element={<div>Welcome to CertiBlock</div>} />
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/certificates/create" element={<CertificateCreateForm />} />
                <Route path="/admin/*" element={<AdminApp />} />
            </Routes>
        </Router>
    );
}

export default App;