import { BrowserRouter as Router, Routes, Route, Link, useLocation, Navigate } from "react-router-dom";
import AdminApp from "./admin/AdminApp";
import { RegisterPage } from "./admin/components/Auth/RegisterPage";
import { CertificateCreateForm } from "./components/Certificates/CertificateCreateForm";
import { useEffect, useState } from "react";
import "./App.css";

function AppContent() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const location = useLocation();
    const isAdminRoute = location.pathname.startsWith('/admin');

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

    // On admin routes, don't show custom header - it's handled by CustomLayout in AdminApp
    if (isAdminRoute) {
        return (
            <Routes>
                <Route path="/admin/*" element={<AdminApp />} />
            </Routes>
        );
    }

    return (
        <>
            <header className="app-header">
                <div className="header-content">
                    <h1 className="app-title">
                        <span className="title-icon">🔗</span>
                        CertiBlock
                    </h1>
                    <nav className="main-nav">
                        <Link to="/" className="nav-btn nav-btn-home">
                            <span className="nav-icon">🏠</span>
                            Home
                        </Link>

                        {!isLoggedIn && (
                            <>
                                <Link to="/admin" className="nav-btn nav-btn-login">
                                    <span className="nav-icon">🔐</span>
                                    Login
                                </Link>
                                <Link to="/register" className="nav-btn nav-btn-register">
                                    <span className="nav-icon">✨</span>
                                    Register
                                </Link>
                            </>
                        )}

                        {isLoggedIn && (
                            <>
                                <Link to="/admin" className="nav-btn nav-btn-admin">
                                    <span className="nav-icon">⚙️</span>
                                    Admin Panel
                                </Link>
                                <Link to="/certificates/create" className="nav-btn nav-btn-create">
                                    <span className="nav-icon">📜</span>
                                    Create Certificate
                                </Link>
                                <button onClick={handleLogout} className="nav-btn nav-btn-logout">
                                    <span className="nav-icon">🚪</span>
                                    Logout
                                </button>
                            </>
                        )}
                    </nav>
                </div>
            </header>

            <main className="app-main">
                <Routes>
                    <Route path="/" element={<Navigate to="/admin" replace />} />
                    <Route path="/register" element={<RegisterPage />} />
                    <Route path="/certificates/create" element={<CertificateCreateForm />} />
                </Routes>
            </main>
        </>
    );
}

function App() {
    return (
        <Router>
            <AppContent />
        </Router>
    );
}

export default App;