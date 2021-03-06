﻿import * as React from 'react'
import * as numbro from 'numbro'
import * as moment from 'moment'
import { Dic } from '../../../../Framework/Signum.React/Scripts/Globals'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import { OrderEntity, CustomerEntity, OrderDetailEmbedded, OrderState } from '../Southwind.Entities'
import { ValueLine, EntityLine, EntityCombo, EntityList, EntityDetail, EntityStrip, EntityRepeater, TypeContext, FormGroup, FormControlStatic, EntityTable, ChangeEvent } from '../../../../Framework/Signum.React/Scripts/Lines'

export default class Order extends React.Component<{ ctx: TypeContext<OrderEntity> }, void> {

    handleCustomerChange = (c: ChangeEvent) => {
        this.props.ctx.value.shipAddress = c.newValue == undefined ? undefined : { ...(c.newValue as CustomerEntity).address };
        this.forceUpdate();
    }


    handleProductChange = (detail: OrderDetailEmbedded) => {

        detail.quantity = 1;
        detail.unitPrice = 0;
        this.forceUpdate();

        if (detail.product)
            Navigator.API.fetchAndForget(detail.product)
                .then(p => detail.unitPrice = p.unitPrice)
                .then(() => this.forceUpdate())
                .done();
    }

    render() {
        const ctx2 = this.props.ctx.subCtx({ labelColumns: { sm: 2 } });
        const ctx4 = this.props.ctx.subCtx({ labelColumns: { sm: 4 } });
        var o = ctx4.value;
        return (
            <div>
                <div className="row">
                    <div className="col-sm-6">
                        <EntityLine ctx={ctx2.subCtx(o => o.customer)} onChange={this.handleCustomerChange} />
                        <EntityDetail ctx={ctx2.subCtx(o => o.shipAddress)} />
                    </div>
                    <div className="col-sm-6">
                        <ValueLine ctx={ctx4.subCtx(o => o.shipName)} />
                        {ctx2.value.isLegacy && < ValueLine ctx={ctx4.subCtx(o => o.isLegacy)} readOnly={true} />}
                        <ValueLine ctx={ctx4.subCtx(o => o.state)} readOnly={true} valueHtmlAttributes={{ style: { color: stateColor(o.state) } }} />
                        <ValueLine ctx={ctx4.subCtx(o => o.orderDate)} unitText={ago(o.orderDate)} readOnly={true} />
                        <ValueLine ctx={ctx4.subCtx(o => o.requiredDate)} unitText={ago(o.requiredDate)} onChange={() => this.forceUpdate()} />
                        <ValueLine ctx={ctx4.subCtx(o => o.shippedDate)} unitText={ago(o.shippedDate)} hideIfNull={true} readOnly={true}  />
                        <ValueLine ctx={ctx4.subCtx(o => o.cancelationDate)} unitText={ago(o.cancelationDate)} hideIfNull={true} readOnly={true}  />
                        <EntityCombo ctx={ctx4.subCtx(o => o.shipVia)} />
                    </div>
                </div>
                <EntityTable ctx={ctx2.subCtx(o => o.details)} onChange={() => this.forceUpdate()} columns={EntityTable.typedColumns<OrderDetailEmbedded>([
                    { property: a => a.product, headerHtmlAttributes: { width: "40%" }, template: dc => <EntityLine ctx={dc.subCtx(a => a.product)} onChange={() => this.handleProductChange(dc.value)} /> },
                    { property: a => a.quantity, headerHtmlAttributes: { width: "15%" }, template: dc => <ValueLine ctx={dc.subCtx(a => a.quantity)} onChange={() => this.forceUpdate()} /> },
                    { property: a => a.unitPrice, headerHtmlAttributes: { width: "15%" }, template: dc => <ValueLine ctx={dc.subCtx(a => a.unitPrice)} readOnly={true} /> },
                    { property: a => a.discount, headerHtmlAttributes: { width: "15%" }, template: dc => <ValueLine ctx={dc.subCtx(a => a.discount)} onChange={() => this.forceUpdate()} /> },
                    {
                        header: "SubTotalPrice", headerHtmlAttributes: { width: "15%" }, template: dc => <FormGroup ctx={dc} labelText="SubTotalPrice">
                            <FormControlStatic ctx={dc}>
                                {numbro(subTotalPrice(dc.value)).format()} €
                            </FormControlStatic>
                        </FormGroup>
                    },    
                ])} />
                <div className="row">
                    <div className="col-sm-4">
                        <EntityLine ctx={ctx4.subCtx(o => o.employee)} />
                    </div>
                    <div className="col-sm-4">
                        <ValueLine ctx={ctx4.subCtx(o => o.freight)} />
                    </div>
                <div className="col-sm-4">
                        <FormGroup ctx={ctx4} labelText="TotalPrice">
                            <FormControlStatic ctx={ctx4}>
                                {numbro(ctx4.value.details.map(mle => subTotalPrice(mle.element)).sum()).format()} €
                            </FormControlStatic>
                        </FormGroup>
                    </div>
                </div>

            </div>
        );
    }
}

function ago(date : string | null | undefined){

    if (!date)
        return undefined;

    return moment(date).fromNow(true);
}

function stateColor(s: OrderState | undefined) {

    if (!s)
        return undefined;

    switch (s) {
        case "New": 
        case "Ordered": return "#33cc33"; 
        case "Shipped": return "#0066ff";
        case "Canceled": return "#ff0000";

    }
}

function subTotalPrice(od: OrderDetailEmbedded) {
    return (od.quantity || 0) * (od.unitPrice || 0) * (1 - (od.discount || 0));
}
