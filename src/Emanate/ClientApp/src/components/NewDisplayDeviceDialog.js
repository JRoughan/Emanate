import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/NewDisplayDeviceDialog';
import Modal from 'react-modal';

class NewDisplayDeviceDialog extends Component {

    constructor(props) {
        super(props);
        this.state = {
            newDisplayDeviceName: '',
            newDisplayTypeId: ''
        };
    }

    handleNameChange = (event) => {
        this.setState({ newDisplayDeviceName: event.target.value });
    }

    handleTypeChange = (event) => {
        this.setState({ newDisplayTypeId: event.target.value });
    }

    handleSubmit = (event) => {
        this.props.addDisplayDevice({
            name: this.state.newDisplayDeviceName,
            typeId: this.state.newDisplayTypeId
        });
        event.preventDefault();
    }

    render() {
        return (
            <div>
                <button className="btn btn-primary" onClick={this.props.openNewDisplayDeviceDialog}>Add Display Device</button>
                <Modal
                    isOpen={this.props.newDisplayDeviceDialogIsOpen}
                    onRequestClose={this.props.closeNewDisplayDeviceDialog}
                    shouldCloseOnOverlayClick={false}
                    style={customStyles}>

                    <form onSubmit={this.handleSubmit}>
                        <div className="form-group">
                            <label htmlFor="newDisplayDeviceName">Name </label>
                            <input type="text" className="form-control" placeholder="My Device" value={this.state.newDisplayDeviceName} onChange={this.handleNameChange} />
                        </div>
                        <div className="form-group">
                            <label htmlFor="newDisplayDeviceName">Name </label>
                            <select className="form-control" value={this.state.newDisplayTypeId} onChange={this.handleTypeChange}>
                                <option hidden>Select Type</option>
                                {this.props.displayDeviceTypes.map(type =>
                                    <option key={type.id} value={type.id}>{type.name}</option>
                                )}
                            </select>
                        </div>
                        <button className="btn btn-secondary" onClick={this.props.closeNewDisplayDeviceDialog}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Add Device</button>
                    </form>
                </Modal>
            </div>
        );
    }
}

const customStyles = {
    content: {
        top: '50%',
        left: '50%',
        right: 'auto',
        bottom: 'auto',
        marginRight: '-50%',
        transform: 'translate(-50%, -50%)'
    }
};

export default connect(
    state => state.newDisplayDeviceDialog,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(NewDisplayDeviceDialog);
