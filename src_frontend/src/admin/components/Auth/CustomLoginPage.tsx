import { Login, LoginForm } from 'react-admin';
import { Box, Typography, Link } from '@mui/material';

export const CustomLoginPage = () => {
    const handleRegisterClick = () => {
        window.location.href = '/register';
    };

    return (
        <Login
            sx={{
                backgroundImage: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            }}
        >
            <Box
                sx={{
                    padding: '2rem',
                    backgroundColor: 'black',
                    borderRadius: '12px',
                    minWidth: '300px'
                }}
            >
                <LoginForm />
                <Box mt={3} textAlign="center">
                    <Typography variant="body2" color="text.secondary">
                        Don't have an account?{' '}
                        <Link
                            component="button"
                            variant="body2"
                            onClick={handleRegisterClick}
                            sx={{
                                cursor: 'pointer',
                                fontWeight: 600,
                                textDecoration: 'none',
                                '&:hover': { textDecoration: 'underline' }
                            }}
                        >
                            Sign Up
                        </Link>
                    </Typography>
                </Box>
            </Box>
        </Login>
    );
};