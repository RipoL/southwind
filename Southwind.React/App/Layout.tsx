
import * as React from 'react'
import { RouteComponentProps } from 'react-router'
import * as moment from 'moment'
import * as numbro from 'numbro'
import { Navbar, Nav, NavItem, NavDropdown, MenuItem } from 'react-bootstrap'
import { LinkContainer } from "react-router-bootstrap";
import { Link } from 'react-router-dom'
import LoginUserControl from '../../Extensions/Signum.React.Extensions/Authorization/Login/LoginUserControl'
import * as AuthClient from '../../Extensions/Signum.React.Extensions/Authorization/AuthClient'
import OmniboxAutocomplete from '../../Extensions/Signum.React.Extensions/Omnibox/OmniboxAutocomplete'
import * as Navigator from "../../Framework/Signum.React/Scripts/Navigator"
import { GlobalModalContainer } from "../../Framework/Signum.React/Scripts/Modals"
import Notify from "../../Framework/Signum.React/Scripts/Frames/Notify"
import ContainerToggle from "../../Framework/Signum.React/Scripts/Frames/ContainerToggle"
import CultureDropdown from "../../Extensions/Signum.React.Extensions/Translation/CultureDropdown"
import * as CultureClient from "../../Extensions/Signum.React.Extensions/Translation/CultureClient"



export default class Layout extends React.Component<void, { refreshId: number }> {

    constructor(props: void) {
        super(props);
        
        this.state = { refreshId: 0 }; 
    }

    static switch: React.ReactElement<any>;
    

    render() {
        return (
            <div id="main" key={this.state.refreshId}>
                <Navbar inverse>
                    <Navbar.Header>
                        <Navbar.Brand>
                            <Link to="~/">Southwind</Link>
                        </Navbar.Brand>
                        <Navbar.Toggle />
                    </Navbar.Header>
                    <Navbar.Collapse>

                        {AuthClient.currentUser() && <ul className="nav navbar-nav">
                            <li>
                                <div style={{ width: "200px", marginTop: "8px" }}>
                                    <OmniboxAutocomplete  inputAttrs={{ className: "form-control" }} />
                                </div>
                            </li>
                            <NavDropdown title="Menu" id="basic-nav-dropdown">
                                <LinkContainer to="~/" exact><MenuItem>Home</MenuItem></LinkContainer>
                                <LinkContainer to="~/publicCatalog"><MenuItem>Orders</MenuItem></LinkContainer>
                                <MenuItem divider />
                                <LinkContainer to="~/find/order"><MenuItem>Orders</MenuItem></LinkContainer>
                                <LinkContainer to="~/find/exception"><MenuItem>Exceptions</MenuItem></LinkContainer>
                            </NavDropdown>
                        </ul>}
                        <ul className="nav navbar-nav navbar-right">
                            <CultureDropdown />
                            <LoginUserControl />
                        </ul>
                    </Navbar.Collapse>
                </Navbar>
                <Notify />
                <ContainerToggle>
                    {Layout.switch}
                </ContainerToggle>
                <GlobalModalContainer />
                <div id="push"></div>
            </div>
        );
    }

    componentWillMount() {
        AuthClient.onCurrentUserChanged.push(this.handleResetUI);
        CultureClient.onCultureLoaded.push(this.handleResetUI);
    }

    componentWillUnmount() {
        AuthClient.onCurrentUserChanged.remove(this.handleResetUI);
        CultureClient.onCultureLoaded.remove(this.handleResetUI);
    }

    handleResetUI = () => {
        this.setState({ refreshId: this.state.refreshId + 1 });
    };
}
