import { useState, useEffect } from 'react';
import {
    Card,
    CardContent,
    Typography,
    Box,
    Grid,
    CircularProgress,
    Alert,
    Chip,
} from '@mui/material';
import AccountBalanceWalletIcon from '@mui/icons-material/AccountBalanceWallet';
import RefreshIcon from '@mui/icons-material/Refresh';
import { IconButton } from '@mui/material';
import { apiEndpoints } from '../../../config.ts';

interface WalletBalance {
    address: string;
    balance: number;
    unit: string;
}

interface WalletConfig {
    name: string;
    address: string;
    blockchain: 'Ethereum' | 'Polygon';
}

const WALLETS: WalletConfig[] = [
    {
        name: 'Primary Wallet ETH',
        address: '0x00BA4d1E704D141003a53d9fACD4459945b29394',
        blockchain: 'Ethereum',
    },
    {
        name: 'Primary Wallet Polygon',
        address: '0x00BA4d1E704D141003a53d9fACD4459945b29394',
        blockchain: 'Polygon',
    },
];

const WalletCard = ({ wallet }: { wallet: WalletConfig }) => {
    const [balance, setBalance] = useState<WalletBalance | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchBalance = async () => {
        setLoading(true);
        setError(null);

        try {
            const endpoint = wallet.blockchain === 'Ethereum'
                ? `getEthBalanceByWalletAddress/${wallet.address}`
                : `getMaticBalanceByWalletAddress/${wallet.address}`;

            const response = await fetch(
                wallet.blockchain === 'Ethereum'
                    ? apiEndpoints.ethereum(endpoint)
                    : apiEndpoints.polygon(endpoint)
            );

            if (!response.ok) {
                throw new Error('Failed to retrieve balance');
            }

            const data = await response.json();
            setBalance(data);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Error');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchBalance();
    }, [wallet.address]);

    return (
        <Card variant="outlined">
            <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', mb: 2 }}>
                    <Box>
                        <Typography variant="h6" gutterBottom>
                            {wallet.name}
                        </Typography>
                        <Chip
                            label={wallet.blockchain}
                            size="small"
                            color={wallet.blockchain === 'Ethereum' ? 'primary' : 'secondary'}
                        />
                    </Box>
                    <IconButton
                        size="small"
                        onClick={fetchBalance}
                        disabled={loading}
                    >
                        <RefreshIcon />
                    </IconButton>
                </Box>

                <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{
                        fontFamily: 'monospace',
                        fontSize: '0.75rem',
                        wordBreak: 'break-all',
                        mb: 2
                    }}
                >
                    {wallet.address}
                </Typography>

                {loading && (
                    <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
                        <CircularProgress size={30} />
                    </Box>
                )}

                {error && (
                    <Alert severity="error" sx={{ py: 0.5 }}>
                        {error}
                    </Alert>
                )}

                {!loading && !error && balance && (
                    <Box sx={{ bgcolor: 'background.default', p: 2, borderRadius: 1 }}>
                        <Typography variant="body2" color="text.secondary">
                            Balance
                        </Typography>
                        <Typography variant="h4" color="primary">
                            {balance.balance.toFixed(6)}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            {balance.unit}
                        </Typography>
                    </Box>
                )}
            </CardContent>
        </Card>
    );
};

export const WalletDashboard = () => {
    return (
        <Box sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
                <AccountBalanceWalletIcon sx={{ fontSize: 40, mr: 2 }} />
                <Typography variant="h4" component="h1">
                    Wallet List
                </Typography>
            </Box>

            <Grid container spacing={3}>
                {WALLETS.map((wallet, index) => (
                    <Grid item xs={12} md={6} lg={4} key={index}>
                        <WalletCard wallet={wallet} />
                    </Grid>
                ))}
            </Grid>
        </Box>
    );
};