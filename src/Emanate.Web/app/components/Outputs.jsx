import React from 'react';
import Editable from './Editable.jsx';

export default ({outputs, onValueClick, onEdit, onDelete}) => {
  return (
    <ul className="outputs">{outputs.map(output =>
      <li className="output" key={output.id}>
        <Editable
          editing={output.editing}
          value={output.name}
          onValueClick={onValueClick.bind(null, output.id)}
          onEdit={onEdit.bind(null, output.id)}
          onDelete={onDelete.bind(null, output.id)} />
          <div>{output.profile.name}</div>
      </li>
    )}</ul>
  );
}
