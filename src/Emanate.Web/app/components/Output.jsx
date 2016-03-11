import React from 'react';
import OutputActions from '../actions/OutputActions';
import Editable from './Editable.jsx';

export default class Output extends React.Component {
    render() {
        const { output, onDelete } = this.props;

        return (
            <div>
                <Editable   editing={output.editing}
                            value={output.name}
                            onValueClick={this.activateOutputEdit}
                            onEdit={this.editOutput}
                            onDelete={onDelete.bind(null, output.id)} />
                <div>{output.profile.name}</div>
            </div>
        );
    }

    editOutput = (name) => {
        const outputId = this.props.output.id;
       OutputActions.update({ id: outputId, name, editing: false });
    };

    activateOutputEdit = () => {
        const outputId = this.props.output.id;
        OutputActions.update({ id: outputId, editing: true });
    };
}



















