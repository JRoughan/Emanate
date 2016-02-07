import React from 'react';

export default class Output extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      editing: false
    };
  }
  render() {
    if(this.state.editing) {
      return this.renderEdit();
    }

    return this.renderOutput();
  }
  renderEdit = () => {
    return <input type="text"
      ref={
        (e) => e ? e.selectionStart = this.props.name.length : null
      }
      autoFocus={true}
      defaultValue={this.props.name}
      onBlur={this.finishEdit}
      onKeyPress={this.checkEnter} />;
  };
  renderDelete = () => {
    return <button
      className="delete-output"
      onClick={this.props.onDelete}>x</button>;
  };
  renderOutput = () => {
    const onDelete = this.props.onDelete;

    return (
      <div onClick={this.edit}>
        <span className="name">{this.props.name}</span>
        {onDelete ? this.renderDelete() : null }
      </div>
    );
  };
  edit = () => {
    this.setState({
      editing: true
    });
  };
  checkEnter = (e) => {
    if(e.key === 'Enter') {
      this.finishEdit(e);
    }
  };
  finishEdit = (e) => {
    const value = e.target.value;

    if(this.props.onEdit && value.trim()) {
      this.props.onEdit(value);

      // Exit edit mode.
      this.setState({
        editing: false
      });
    }
  };
}
