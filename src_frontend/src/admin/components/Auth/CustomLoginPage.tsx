import { Login, LoginForm } from 'react-admin';
import { Box, Typography, Link } from '@mui/material';
import { LockOpen as LockOpenIcon } from '@mui/icons-material';

export const CustomLoginPage = () => {
    const handleRegisterClick = () => {
        window.location.href = '/register';
    };

    return (
        <Login
            sx={{
                backgroundImage: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)',
                minHeight: '100vh',
                '& .RaLogin-card': {
                    backgroundColor: 'rgba(26, 26, 46, 0.95)',
                    backdropFilter: 'blur(20px)',
                    borderRadius: '24px',
                    border: '1px solid rgba(255, 255, 255, 0.1)',
                    boxShadow: '0 25px 50px rgba(0, 0, 0, 0.5), 0 0 100px rgba(139, 92, 246, 0.1)',
                    padding: '2rem',
                    minWidth: '380px',
                },
                '& .MuiTextField-root': {
                    marginBottom: '1rem',
                    '& .MuiOutlinedInput-root': {
                        borderRadius: '12px',
                        backgroundColor: 'rgba(255, 255, 255, 0.05)',
                        transition: 'all 0.3s ease',
                        '&:hover': {
                            backgroundColor: 'rgba(255, 255, 255, 0.08)',
                        },
                        '&.Mui-focused': {
                            backgroundColor: 'rgba(139, 92, 246, 0.1)',
                            '& .MuiOutlinedInput-notchedOutline': {
                                borderColor: '#8b5cf6',
                                borderWidth: '2px',
                            },
                        },
                    },
                    '& .MuiInputLabel-root': {
                        color: 'rgba(255, 255, 255, 0.7)',
                    },
                    '& .MuiOutlinedInput-input': {
                        color: '#fff',
                    },
                },
                '& .MuiButton-contained': {
                    background: 'linear-gradient(135deg, #8b5cf6 0%, #6d28d9 100%)',
                    borderRadius: '12px',
                    padding: '12px 24px',
                    fontSize: '1rem',
                    fontWeight: 600,
                    textTransform: 'none',
                    boxShadow: '0 4px 20px rgba(139, 92, 246, 0.4)',
                    transition: 'all 0.3s ease',
                    '&:hover': {
                        background: 'linear-gradient(135deg, #7c3aed 0%, #5b21b6 100%)',
                        transform: 'translateY(-2px)',
                        boxShadow: '0 6px 25px rgba(139, 92, 246, 0.6)',
                    },
                },
            }}
        >
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    mb: 3,
                }}
            >
                <Box
                    sx={{
                        width: 70,
                        height: 70,
                        borderRadius: '20px',
                        background: 'linear-gradient(135deg, #8b5cf6 0%, #6d28d9 100%)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        mb: 2,
                        boxShadow: '0 10px 30px rgba(139, 92, 246, 0.4)',
                        animation: 'pulse 2s ease-in-out infinite',
                        '@keyframes pulse': {
                            '0%, 100%': { transform: 'scale(1)' },
                            '50%': { transform: 'scale(1.05)' },
                        },
                    }}
                >
                    <LockOpenIcon sx={{ color: 'white', fontSize: 36 }} />
                </Box>
                <Typography
                    variant="h4"
                    sx={{
                        fontWeight: 700,
                        background: 'linear-gradient(90deg, #00d4ff, #7c3aed, #f472b6)',
                        backgroundSize: '200% 200%',
                        WebkitBackgroundClip: 'text',
                        WebkitTextFillColor: 'transparent',
                        backgroundClip: 'text',
                        animation: 'gradient-shift 3s ease infinite',
                        '@keyframes gradient-shift': {
                            '0%, 100%': { backgroundPosition: '0% 50%' },
                            '50%': { backgroundPosition: '100% 50%' },
                        },
                    }}
                >
                    Welcome Back
                </Typography>
                <Typography
                    variant="body2"
                    sx={{
                        color: 'rgba(255, 255, 255, 0.6)',
                        mt: 1,
                    }}
                >
                    Sign in to your CertiBlock account
                </Typography>
            </Box>

            <LoginForm />

            <Box mt={3} textAlign="center">
                <Typography variant="body2" sx={{ color: 'rgba(255, 255, 255, 0.6)' }}>
                    Don't have an account?{' '}
                    <Link
                        component="button"
                        variant="body2"
                        onClick={handleRegisterClick}
                        sx={{
                            cursor: 'pointer',
                            fontWeight: 600,
                            color: '#8b5cf6',
                            textDecoration: 'none',
                            transition: 'all 0.3s ease',
                            '&:hover': {
                                color: '#a78bfa',
                                textDecoration: 'underline'
                            }
                        }}
                    >
                        Sign Up
                    </Link>
                </Typography>
            </Box>
        </Login>
    );
};