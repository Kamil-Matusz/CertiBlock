import { AppBar, TitlePortal } from 'react-admin';
import { Link } from 'react-router-dom';
import { Box, Button } from '@mui/material';
import HomeIcon from '@mui/icons-material/Home';
import AddIcon from '@mui/icons-material/Add';

export const CustomAppBar = () => (
    <AppBar>
        <TitlePortal />
        <Box sx={{ flex: 1 }} />
        <Button
            component={Link}
            to="/"
            color="inherit"
            startIcon={<HomeIcon />}
            sx={{ mr: 1 }}
        >
            Home
        </Button>
        <Button
            component={Link}
            to="/certificates/create"
            color="inherit"
            startIcon={<AddIcon />}
            sx={{ mr: 2 }}
        >
            Create Certificate
        </Button>
    </AppBar>
);