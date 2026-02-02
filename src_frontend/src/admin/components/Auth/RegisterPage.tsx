import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Card,
    CardContent,
    TextField,
    Button,
    Typography,
    Alert,
    Link,
    FormControlLabel,
    Checkbox,
    MenuItem
} from '@mui/material';
import { PersonAdd as PersonAddIcon } from '@mui/icons-material';
import { API_URL } from '../../../config.ts';

export const RegisterPage = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        confirmPassword: '',
        role: 'User',
        isActive: true
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);

    const handleChange = (field: string) => (event: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [field]: event.target.value
        });
        setError('');
    };

    const handleCheckboxChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            isActive: event.target.checked
        });
    };

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();

        if (!formData.email || !formData.password) {
            setError('Email and password are required');
            return;
        }

        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        if (formData.password.length < 6) {
            setError('Password must be at least 6 characters long');
            return;
        }

        if (formData.password.length > 200) {
            setError('Password cannot be longer than 200 characters');
            return;
        }

        setLoading(true);
        setError('');

        try {
            const response = await fetch(`${API_URL}/users-service/Users/signUp`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    userId: '00000000-0000-0000-0000-000000000000',
                    email: formData.email,
                    password: formData.password,
                    role: formData.role,
                    isActive: formData.isActive
                }),
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || 'Registration failed');
            }

            setSuccess(true);
            setTimeout(() => {
                navigate('/admin');
            }, 2000);

        } catch (err) {
            setError(err instanceof Error ? err.message : 'Registration failed. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    const handleLoginClick = () => {
        navigate('/admin');
    };

    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                minHeight: '100vh',
                background: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)',
                padding: 2,
                position: 'relative',
                overflow: 'hidden',
                '&::before': {
                    content: '""',
                    position: 'absolute',
                    top: '-50%',
                    left: '-50%',
                    width: '200%',
                    height: '200%',
                    background: 'radial-gradient(circle at 30% 30%, rgba(6, 182, 212, 0.1) 0%, transparent 50%), radial-gradient(circle at 70% 70%, rgba(139, 92, 246, 0.1) 0%, transparent 50%)',
                    animation: 'rotate 30s linear infinite',
                    '@keyframes rotate': {
                        '0%': { transform: 'rotate(0deg)' },
                        '100%': { transform: 'rotate(360deg)' },
                    },
                },
            }}
        >
            <Card
                sx={{
                    maxWidth: 480,
                    width: '100%',
                    borderRadius: '24px',
                    boxShadow: '0 25px 50px rgba(0, 0, 0, 0.5), 0 0 100px rgba(6, 182, 212, 0.1)',
                    backgroundColor: 'rgba(26, 26, 46, 0.95)',
                    backdropFilter: 'blur(20px)',
                    border: '1px solid rgba(255, 255, 255, 0.1)',
                    position: 'relative',
                    zIndex: 1,
                }}
            >
                <CardContent sx={{ p: 4 }}>
                    <Box display="flex" flexDirection="column" alignItems="center" mb={3}>
                        <Box
                            sx={{
                                width: 70,
                                height: 70,
                                borderRadius: '20px',
                                background: 'linear-gradient(135deg, #06b6d4 0%, #0891b2 100%)',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                mb: 2,
                                boxShadow: '0 10px 30px rgba(6, 182, 212, 0.4)',
                                animation: 'pulse 2s ease-in-out infinite',
                                '@keyframes pulse': {
                                    '0%, 100%': { transform: 'scale(1)' },
                                    '50%': { transform: 'scale(1.05)' },
                                },
                            }}
                        >
                            <PersonAddIcon sx={{ color: 'white', fontSize: 36 }} />
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
                            Create Account
                        </Typography>
                        <Typography variant="body2" sx={{ color: 'rgba(255, 255, 255, 0.6)', mt: 1 }}>
                            Join CertiBlock and start securing your certificates
                        </Typography>
                    </Box>

                    {error && (
                        <Alert
                            severity="error"
                            sx={{
                                mb: 3,
                                borderRadius: '12px',
                                backgroundColor: 'rgba(239, 68, 68, 0.1)',
                                border: '1px solid rgba(239, 68, 68, 0.3)',
                                '& .MuiAlert-icon': { color: '#ef4444' },
                                color: '#fca5a5',
                            }}
                        >
                            {error}
                        </Alert>
                    )}

                    {success && (
                        <Alert
                            severity="success"
                            sx={{
                                mb: 3,
                                borderRadius: '12px',
                                backgroundColor: 'rgba(16, 185, 129, 0.1)',
                                border: '1px solid rgba(16, 185, 129, 0.3)',
                                '& .MuiAlert-icon': { color: '#10b981' },
                                color: '#6ee7b7',
                            }}
                        >
                            Registration successful! Redirecting to login...
                        </Alert>
                    )}

                    <form onSubmit={handleSubmit}>
                        <TextField
                            label="Email"
                            type="email"
                            fullWidth
                            required
                            value={formData.email}
                            onChange={handleChange('email')}
                            margin="normal"
                            disabled={loading || success}
                            autoComplete="email"
                            sx={{
                                '& .MuiOutlinedInput-root': {
                                    borderRadius: '12px',
                                    backgroundColor: 'rgba(255, 255, 255, 0.05)',
                                    transition: 'all 0.3s ease',
                                    '&:hover': {
                                        backgroundColor: 'rgba(255, 255, 255, 0.08)',
                                    },
                                    '&.Mui-focused': {
                                        backgroundColor: 'rgba(6, 182, 212, 0.1)',
                                        '& .MuiOutlinedInput-notchedOutline': {
                                            borderColor: '#06b6d4',
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
                                '& .MuiFormHelperText-root': {
                                    color: 'rgba(255, 255, 255, 0.5)',
                                },
                            }}
                        />

                        <TextField
                            label="Password"
                            type="password"
                            fullWidth
                            required
                            value={formData.password}
                            onChange={handleChange('password')}
                            margin="normal"
                            disabled={loading || success}
                            autoComplete="new-password"
                            helperText="Minimum 6 characters"
                            sx={{
                                '& .MuiOutlinedInput-root': {
                                    borderRadius: '12px',
                                    backgroundColor: 'rgba(255, 255, 255, 0.05)',
                                    transition: 'all 0.3s ease',
                                    '&:hover': {
                                        backgroundColor: 'rgba(255, 255, 255, 0.08)',
                                    },
                                    '&.Mui-focused': {
                                        backgroundColor: 'rgba(6, 182, 212, 0.1)',
                                        '& .MuiOutlinedInput-notchedOutline': {
                                            borderColor: '#06b6d4',
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
                                '& .MuiFormHelperText-root': {
                                    color: 'rgba(255, 255, 255, 0.5)',
                                },
                            }}
                        />

                        <TextField
                            label="Confirm Password"
                            type="password"
                            fullWidth
                            required
                            value={formData.confirmPassword}
                            onChange={handleChange('confirmPassword')}
                            margin="normal"
                            disabled={loading || success}
                            autoComplete="new-password"
                            sx={{
                                '& .MuiOutlinedInput-root': {
                                    borderRadius: '12px',
                                    backgroundColor: 'rgba(255, 255, 255, 0.05)',
                                    transition: 'all 0.3s ease',
                                    '&:hover': {
                                        backgroundColor: 'rgba(255, 255, 255, 0.08)',
                                    },
                                    '&.Mui-focused': {
                                        backgroundColor: 'rgba(6, 182, 212, 0.1)',
                                        '& .MuiOutlinedInput-notchedOutline': {
                                            borderColor: '#06b6d4',
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
                            }}
                        />

                        <TextField
                            label="Role"
                            select
                            fullWidth
                            value={formData.role}
                            onChange={handleChange('role')}
                            margin="normal"
                            disabled={loading || success}
                            sx={{
                                '& .MuiOutlinedInput-root': {
                                    borderRadius: '12px',
                                    backgroundColor: 'rgba(255, 255, 255, 0.05)',
                                    transition: 'all 0.3s ease',
                                    '&:hover': {
                                        backgroundColor: 'rgba(255, 255, 255, 0.08)',
                                    },
                                    '&.Mui-focused': {
                                        backgroundColor: 'rgba(6, 182, 212, 0.1)',
                                        '& .MuiOutlinedInput-notchedOutline': {
                                            borderColor: '#06b6d4',
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
                                '& .MuiSelect-icon': {
                                    color: 'rgba(255, 255, 255, 0.7)',
                                },
                            }}
                        >
                            <MenuItem value="User">User</MenuItem>
                            <MenuItem value="Admin">Admin</MenuItem>
                        </TextField>

                        <FormControlLabel
                            control={
                                <Checkbox
                                    checked={formData.isActive}
                                    onChange={handleCheckboxChange}
                                    disabled={loading || success}
                                    sx={{
                                        color: 'rgba(255, 255, 255, 0.5)',
                                        '&.Mui-checked': {
                                            color: '#06b6d4',
                                        },
                                    }}
                                />
                            }
                            label="Active Account"
                            sx={{
                                mt: 2,
                                mb: 2,
                                '& .MuiFormControlLabel-label': {
                                    color: 'rgba(255, 255, 255, 0.7)',
                                },
                            }}
                        />

                        <Button
                            type="submit"
                            variant="contained"
                            fullWidth
                            size="large"
                            disabled={loading || success}
                            sx={{
                                mt: 2,
                                py: 1.5,
                                borderRadius: '12px',
                                fontSize: '1rem',
                                fontWeight: 600,
                                textTransform: 'none',
                                background: 'linear-gradient(135deg, #06b6d4 0%, #0891b2 100%)',
                                boxShadow: '0 4px 20px rgba(6, 182, 212, 0.4)',
                                transition: 'all 0.3s ease',
                                '&:hover': {
                                    background: 'linear-gradient(135deg, #0891b2 0%, #0e7490 100%)',
                                    transform: 'translateY(-2px)',
                                    boxShadow: '0 6px 25px rgba(6, 182, 212, 0.6)',
                                },
                                '&:disabled': {
                                    background: 'rgba(255, 255, 255, 0.1)',
                                    color: 'rgba(255, 255, 255, 0.3)',
                                },
                            }}
                        >
                            {loading ? 'Creating Account...' : 'Create Account'}
                        </Button>
                    </form>

                    <Box mt={3} textAlign="center">
                        <Typography variant="body2" sx={{ color: 'rgba(255, 255, 255, 0.6)' }}>
                            Already have an account?{' '}
                            <Link
                                component="button"
                                variant="body2"
                                onClick={handleLoginClick}
                                sx={{
                                    cursor: 'pointer',
                                    fontWeight: 600,
                                    color: '#06b6d4',
                                    textDecoration: 'none',
                                    transition: 'all 0.3s ease',
                                    '&:hover': {
                                        color: '#22d3ee',
                                        textDecoration: 'underline'
                                    }
                                }}
                            >
                                Sign In
                            </Link>
                        </Typography>
                    </Box>
                </CardContent>
            </Card>
        </Box>
    );
};