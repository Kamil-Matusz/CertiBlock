import { useState } from 'react';
import {
    Card,
    CardContent,
    TextField,
    Button,
    Typography,
    Box,
    CircularProgress,
    Alert,
    Tabs,
    Tab,
} from '@mui/material';
import AccountBalanceWalletIcon from '@mui/icons-material/AccountBalanceWallet';
import SearchIcon from '@mui/icons-material/Search';
import { API_URL } from '../../../config.ts';

interface WalletBalance {
    address: string;
    balance: number;
    unit: string;
}

interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
}

function TabPanel(props: TabPanelProps) {
    const { children, value, index, ...other } = props;
    return (
        <div hidden={value !== index} {...other}>
            {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
        </div>
    );
}

export const WalletBalanceChecker = () => {
    const [tabValue, setTabValue] = useState(0);
    const [ethAddress, setEthAddress] = useState('');
    const [polygonAddress, setPolygonAddress] = useState('');
    const [ethBalance, setEthBalance] = useState<WalletBalance | null>(null);
    const [polygonBalance, setPolygonBalance] = useState<WalletBalance | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchEthBalance = async () => {
        if (!ethAddress) {
            setError('Enter your wallet address');
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const response = await fetch(
                `${API_URL}/ethereum-service/Ethereum/getEthBalanceByWalletAddress/${ethAddress}`
            );

            if (!response.ok) {
                throw new Error('Failed to retrieve balance');
            }

            const data = await response.json();
            setEthBalance(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Error');
        } finally {
            setLoading(false);
        }
    };

    const fetchPolygonBalance = async () => {
        if (!polygonAddress) {
            setError('Enter your wallet address');
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const response = await fetch(
                `${API_URL}/polygon-service/Polygon/getPolygonBalanceByWalletAddress/${polygonAddress}`
            );

            if (!response.ok) {
                throw new Error('Failed to retrieve balance');
            }

            const data = await response.json();
            setPolygonBalance(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Error');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Card sx={{ maxWidth: 800, margin: 'auto', mt: 4 }}>
            <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
                    <AccountBalanceWalletIcon sx={{ fontSize: 40, mr: 2 }} />
                    <Typography variant="h5" component="h2">
                        Check Wallet Balance
                    </Typography>
                </Box>

                <Tabs value={tabValue} onChange={(_, newValue) => setTabValue(newValue)}>
                    <Tab label="Ethereum" />
                    <Tab label="Polygon" />
                </Tabs>

                {/* Ethereum Tab */}
                <TabPanel value={tabValue} index={0}>
                    <Box sx={{ display: 'flex', gap: 2, mb: 3 }}>
                        <TextField
                            fullWidth
                            label="Ethereum Wallet Address"
                            value={ethAddress}
                            onChange={(e) => setEthAddress(e.target.value)}
                            placeholder="0x..."
                            variant="outlined"
                        />
                        <Button
                            variant="contained"
                            onClick={fetchEthBalance}
                            disabled={loading}
                            startIcon={loading ? <CircularProgress size={20} /> : <SearchIcon />}
                            sx={{ minWidth: 120 }}
                        >
                            Check
                        </Button>
                    </Box>

                    {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

                    {ethBalance && (
                        <Card variant="outlined" sx={{ p: 2, bgcolor: 'background.default' }}>
                            <Typography variant="body2" color="text.secondary">
                                Address
                            </Typography>
                            <Typography
                                variant="body1"
                                sx={{
                                    fontFamily: 'monospace',
                                    wordBreak: 'break-all',
                                    mb: 2
                                }}
                            >
                                {ethBalance.address}
                            </Typography>

                            <Typography variant="body2" color="text.secondary">
                                Balance
                            </Typography>
                            <Typography variant="h4" color="primary">
                                {ethBalance.balance.toFixed(6)} {ethBalance.unit}
                            </Typography>
                        </Card>
                    )}
                </TabPanel>

                {/* Polygon Tab */}
                <TabPanel value={tabValue} index={1}>
                    <Box sx={{ display: 'flex', gap: 2, mb: 3 }}>
                        <TextField
                            fullWidth
                            label="Polygon Wallet Address"
                            value={polygonAddress}
                            onChange={(e) => setPolygonAddress(e.target.value)}
                            placeholder="0x..."
                            variant="outlined"
                        />
                        <Button
                            variant="contained"
                            onClick={fetchPolygonBalance}
                            disabled={loading}
                            startIcon={loading ? <CircularProgress size={20} /> : <SearchIcon />}
                            sx={{ minWidth: 120 }}
                        >
                            Check
                        </Button>
                    </Box>

                    {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

                    {polygonBalance && (
                        <Card variant="outlined" sx={{ p: 2, bgcolor: 'background.default' }}>
                            <Typography variant="body2" color="text.secondary">
                                Address
                            </Typography>
                            <Typography
                                variant="body1"
                                sx={{
                                    fontFamily: 'monospace',
                                    wordBreak: 'break-all',
                                    mb: 2
                                }}
                            >
                                {polygonBalance.address}
                            </Typography>

                            <Typography variant="body2" color="text.secondary">
                                Balance
                            </Typography>
                            <Typography variant="h4" color="primary">
                                {polygonBalance.balance.toFixed(6)} {polygonBalance.unit}
                            </Typography>
                        </Card>
                    )}
                </TabPanel>
            </CardContent>
        </Card>
    );
};