import "./ChatPage.css";
import Chat from "../../Components/Chat/Chat";
import { useParams } from "react-router-dom"

export default function ChatPage({currentUser, currentUserRole, users}) {

  let {username} = useParams();

  if(username === currentUser) {
    
    if(currentUserRole === "administrator") {
      return (

          <div>
            <div>
              {users.map((user) => (
                (user.role !== "administrator") &&
                (<div key={user.id} className="adminChats">
                  <div>{user.username}</div>
                  <Chat currentUser={currentUser} username={username} to={user.username}/>
                </div>)

              ))}
            </div>
          </div>

      );
    }
    else {

      return (

        <div>
          <Chat currentUser={currentUser} username={username} to="admin"/>
        </div>

      );
    }
  }
  else {

      return (

        <div className="notLoggedInMessage">You are not logged in!</div>
      )
  }
}