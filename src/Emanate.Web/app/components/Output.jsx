import React from 'react';
import {compose} from 'redux';

class Output extends React.Component {
  render() {
    const {id, editing, ...props} = this.props;
    return (
      <li {...props}>{props.children}</li>
    );
  }
}

export default compose()(Output);
