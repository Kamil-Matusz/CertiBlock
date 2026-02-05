import { Admin, Resource } from 'react-admin';
import { dataProvider } from './dataProvider';
import { authProvider } from './authProvider';
import { CertificateList } from './components/Certificates/CertificateList.tsx';
import { CertificateShow } from './components/Certificates/CertificateShow.tsx';
import { EthereumTransactionList } from './components/Ethereum/EthereumTransactionList';
import { EthereumTransactionShow } from './components/Ethereum/EthereumTransactionShow';
import { EthereumResearchMetricsList } from './components/Ethereum/EthereumResearchMetricsList';
import { PolygonTransactionList } from './components/Polygon/PolygonTransactionList';
import { PolygonTransactionShow } from './components/Polygon/PolygonTransactionShow';
import { PolygonResearchMetricsList } from './components/Polygon/PolygonResearchMetricsList';
import { WalletBalanceChecker } from './components/Wallets/WalletBalanceChecker';
import { CustomLoginPage } from './components/Auth/CustomLoginPage';
import { WalletDashboard } from './components/Wallets/WalletDashboard';
import { CustomLayout } from './components/Layout/CustomLayout';
import CardMembershipIcon from '@mui/icons-material/CardMembership';
import AccountBalanceIcon from '@mui/icons-material/AccountBalance';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import MonetizationOnIcon from '@mui/icons-material/MonetizationOn';
import AssessmentIcon from '@mui/icons-material/Assessment';

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
                show={EthereumTransactionShow}
                icon={AccountBalanceIcon}
                options={{ label: 'Ethereum Transactions' }}
            />
            <Resource
                name="polygon-transactions"
                list={PolygonTransactionList}
                show={PolygonTransactionShow}
                icon={AccountBalanceIcon}
                options={{ label: 'Polygon Transactions' }}
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
            <Resource
                name="ethereum-research-metrics"
                list={EthereumResearchMetricsList}
                icon={AssessmentIcon}
                options={{ label: 'Ethereum Research Metrics' }}
            />
            <Resource
                name="polygon-research-metrics"
                list={PolygonResearchMetricsList}
                icon={AssessmentIcon}
                options={{ label: 'Polygon Research Metrics' }}
            />
        </Admin>
    );
}