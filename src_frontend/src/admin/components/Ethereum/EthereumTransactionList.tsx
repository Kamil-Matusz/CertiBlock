import {
    List,
    Datagrid,
    TextField,
    DateField,
    FunctionField,
    DeleteButton
} from 'react-admin';
import { Chip } from '@mui/material';

const getStatusColor = (status?: string): 'success' | 'warning' | 'error' | 'default' => {
    switch (status) {
        case 'Confirmed':
            return 'success';
        case 'Pending':
            return 'warning';
        case 'Failed':
            return 'error';
        case 'Submitted':
            return 'warning';
        default:
            return 'default';
    }
};

export const EthereumTransactionList = () => (
    <List>
        <Datagrid bulkActionButtons={false}>
            <TextField source="certificateId" label="Certificate ID" />
            <TextField
                source="transactionHash"
                label="Transaction Hash"
                sx={{
                    maxWidth: '200px',
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap',
                    fontFamily: 'monospace'
                }}
            />
            <FunctionField
                label="Transaction Status"
                render={(record: { status?: string }) => (
                    <Chip
                        label={record?.status || 'Unknown'}
                        color={getStatusColor(record?.status)}
                        size="small"
                    />
                )}
            />
            <DateField source="createdAt" label="Created At" showTime />
            <DeleteButton />
        </Datagrid>
    </List>
);