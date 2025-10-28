import { Admin, Resource } from 'react-admin';
import { dataProvider } from './dataProvider';
import { CertificateList } from './components/Certificates/CertificateList.tsx';
import { CertificateShow } from './components/Certificates/CertificateShow.tsx';
import { EthereumTransactionList } from './components/Ethereum/EthereumTransactionList';
import { EthereumTransactionShow } from './components//Ethereum/EthereumTransactionShow.tsx';
import { PolygonTransactionList } from './components/Polygon/PolygonTransactionList';
import CardMembershipIcon from '@mui/icons-material/CardMembership';
import AccountBalanceIcon from '@mui/icons-material/AccountBalance';

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
        </Admin>
    );
}