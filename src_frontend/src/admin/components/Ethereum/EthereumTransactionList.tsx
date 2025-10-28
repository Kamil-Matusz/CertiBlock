import {
    List,
    Datagrid,
    TextField,
    DateField,
    ChipField,
} from 'react-admin';

const StatusField = ({ record }: any) => {
    const colors: Record<string, string> = {
        'Confirmed': 'success',
        'Pending': 'warning',
        'Failed': 'error',
    };

    return (
        <ChipField
            source="status"
            color={colors[record?.status] || 'default'}
        />
    );
};

export const EthereumTransactionList = () => (
    <List>
        <Datagrid rowClick="show" bulkActionButtons={false}>
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
            <StatusField label="Status" />
            <DateField source="createdAt" label="Created At" showTime />
        </Datagrid>
    </List>
);