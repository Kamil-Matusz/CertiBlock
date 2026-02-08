import { Admin, Resource } from 'react-admin';
import { dataProvider } from './dataProvider';
import { authProvider } from './authProvider';
import { CertificateList } from './components/Certificates/CertificateList.tsx';
import { CertificateShow } from './components/Certificates/CertificateShow.tsx';
import { EthereumTransactionList } from './components/Ethereum/EthereumTransactionList';
import { EthereumResearchMetrics } from './components/Ethereum/EthereumResearchMetrics';
import { PolygonTransactionList } from './components/Polygon/PolygonTransactionList';
import { PolygonResearchMetrics } from './components/Polygon/PolygonResearchMetrics';
import { WalletBalanceChecker } from './components/Wallets/WalletBalanceChecker';
import { CustomLoginPage } from './components/Auth/CustomLoginPage';
import { WalletDashboard } from './components/Wallets/WalletDashboard';
import { CustomLayout } from './components/Layout/CustomLayout';
import CardMembershipIcon from '@mui/icons-material/CardMembership';
import AccountBalanceIcon from '@mui/icons-material/AccountBalance';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import MonetizationOnIcon from '@mui/icons-material/MonetizationOn';
import BarChartIcon from '@mui/icons-material/BarChart';


export default function AdminApp() {
    return (
        <Admin basename="/admin"
            dataProvider={dataProvider}
            authProvider={authProvider}
            loginPage={CustomLoginPage}
            layout={CustomLayout}
        >
            <Resource
                name="certificates"
                list={CertificateList}
                show={CertificateShow}
                icon={CardMembershipIcon}
                options={{ label: 'Certificates' }}
            />
            <Resource
                name="ethereum-transactions"
                list={EthereumTransactionList}
                icon={AccountBalanceIcon}
                options={{ label: 'Ethereum Transactions' }}
            />
            <Resource
                name="ethereum-research-metrics"
                list={EthereumResearchMetrics}
                icon={BarChartIcon}
                options={{ label: 'Ethereum Research Metrics' }}
            />
            <Resource
                name="polygon-transactions"
                list={PolygonTransactionList}
                icon={AccountBalanceIcon}
                options={{ label: 'Polygon Transactions' }}
            />
            <Resource
                name="polygon-research-metrics"
                list={PolygonResearchMetrics}
                icon={BarChartIcon}
                options={{ label: 'Polygon Research Metrics' }}
            />

            <Resource
                name="wallet-checker"
                list={WalletBalanceChecker}
                icon={AttachMoneyIcon}
                options={{ label: 'Check Balance on the Wallet' }}
            />
            <Resource
                name="wallet-dashboard"
                list={WalletDashboard}
                icon={MonetizationOnIcon}
                options={{ label: 'Wallet List' }}
            />
        </Admin>
    );
}