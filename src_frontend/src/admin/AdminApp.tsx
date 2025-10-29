import { Admin, Resource } from 'react-admin';
import { dataProvider } from './dataProvider';
import { CertificateList } from './components/Certificates/CertificateList.tsx';
import { CertificateShow } from './components/Certificates/CertificateShow.tsx';
import { EthereumTransactionList } from './components/Ethereum/EthereumTransactionList';
import { EthereumTransactionShow } from './components//Ethereum/EthereumTransactionShow.tsx';
import { PolygonTransactionList } from './components/Polygon/PolygonTransactionList';
import { WalletBalanceChecker } from './components/Wallets/WalletBalanceChecker';
import { WalletDashboard } from './components/Wallets/WalletDashboard';
import CardMembershipIcon from '@mui/icons-material/CardMembership';
import AccountBalanceIcon from '@mui/icons-material/AccountBalance';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import MonetizationOnIcon from '@mui/icons-material/MonetizationOn';

export default function AdminApp() {
    return (
        <Admin basename="/admin" dataProvider={dataProvider}>
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
                show={EthereumTransactionShow}
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
        </Admin>
    );
}