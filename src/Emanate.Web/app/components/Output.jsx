import React from 'react';
import Modal from 'react-modal';
import OutputActions from '../actions/OutputActions';
import Editable from './Editable.jsx';

export default class Output extends React.Component {

    state = {
        modalIsOpen : false
    };

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
                <button onClick={this.openModal}>Inputs</button>
                <Modal isOpen={this.state.modalIsOpen} onRequestClose={this.closeModal} >
                    <h2>{output.name} inputs<span><button onClick={this.closeModal}>x</button></span></h2>
                    <button>Save</button>
                    <button onClick={this.closeModal}>Cancel</button>
                </Modal>
            </div>
        );
    }

    openModal = () => {
        this.setState({modalIsOpen: true});
    };
     
    closeModal = () => {
        this.setState({modalIsOpen: false});
    };

    editOutput = (name) => {
        const outputId = this.props.output.id;
       OutputActions.update({ id: outputId, name, editing: false });
    };

    activateOutputEdit = () => {
        const outputId = this.props.output.id;
        OutputActions.update({ id: outputId, editing: true });
    };
}



















