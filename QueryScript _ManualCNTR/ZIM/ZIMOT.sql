  (SELECT
 ECM.VESSEL_NM_AN,
 ECM.VOYAGE_AN,
 substr(replace(nvl(translate(ECM.VISIT_VSL_CALL_SIGN_C,'!@#$%^&*-+|\/[]{}<>?_',' '),''),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,
 ECM.CNTR_OPERATOR_CODE,
 substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\/[]{}<>?', '                    '),1,35)AS SHIPPER,
 ECM.CNTR_AN,
 ECM.CONTAINER_TYPE_CODE,
 nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,15),'NOSEAL')AS SEAL,
 decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,
 decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,
 nvl(decode(ECM.LADEN_INDICATOR_AN,'E',(SELECT T.BOOKING_AN FROM TMS_CNTR_GRP1 C, TMS_TRML_DOC_REF T WHERE C.terminal_doc_ref_an= t.terminal_doc_ref_an(+) and c.CNTR_AN=ECM.CNTR_AN)),'NOBOOKING')AS BOOKING_NO_AN, 
 nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,
 to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,
 decode(nvl((select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE), ' '),'MYPKL','MYPKG','VNHCM','VNSGN','CNYAH','CNYAN',nvl((select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE), '')) AS POD,
 decode(nvl((select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE), ' '),'MYPKL','MYPKG','VNHCM','VNSGN','CNYAH','CNYAN',nvl((select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE), '')) AS POL,
 (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,
 decode(nvl(ECM.DST_CODE,''),'LICD','BKK10','SCT','BKK07','LCIT','LCB05','ECT','BKK08','LCD','LCB03','TIPS','LCB02','MON','BKK03','LCT','BKK12','BKK50') AS DST_CODE,
 decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,
  NVL(to_char(ECM.VGM_GROSS_WEIGHT),'NOVGM') AS VGM,
 nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,
 nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,
 (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,
 decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,
 replace(replace(nvl(ECM.INLAND_CARR_CODE, ' '),'&','AND'),'+','AND')AS INLAND_CARR_CODE,
 replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,
 decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,
 decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C
 FROM TMS_OWNER.EDI_CNTR_MOVE ECM
 WHERE ECM.CNTR_OPERATOR_CODE IN ('ZIM')
 AND ECM.MOVE_TYPE_AN IN ('GO','RO')
 AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')
 AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')  
 AND ECM.AREA_C LIKE :BERTH 
 AND ECM.CNTR_AN IN (:CNTR)
 )
 union all
 (SELECT
 ECM.VESSEL_NM_AN,
 ECM.VOYAGE_AN,
 substr(replace(nvl(translate(ECM.VISIT_VSL_CALL_SIGN_C,'!@#$%^&*-+|\/[]{}<>?_',' '),''),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,
 ECM.CNTR_OPERATOR_CODE,
 substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\/[]{}<>?', '                    '),1,35)AS SHIPPER,
 ECM.CNTR_AN,
 ECM.CONTAINER_TYPE_CODE,
 nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,15),'NOSEAL')AS SEAL,
 decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,
 decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,
 nvl(decode(ECM.LADEN_INDICATOR_AN,'E',(SELECT T.BOOKING_AN FROM TMS_CNTR_GRP1 C, TMS_TRML_DOC_REF T WHERE C.terminal_doc_ref_an= t.terminal_doc_ref_an(+) and c.CNTR_AN=ECM.CNTR_AN)),'NOBOOKING')AS BOOKING_NO_AN, 
 nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,
 to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,
  decode(nvl((select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE), ' '),'MYPKL','MYPKG','VNHCM','VNSGN','CNYAH','CNYAN',nvl((select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE), '')) AS POD,
  decode(nvl((select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE), ' '),'MYPKL','MYPKG','VNHCM','VNSGN','CNYAH','CNYAN',nvl((select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE), ''))AS POL,
 (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,
  decode(nvl(ECM.DST_CODE,''),'LICD','BKK10','SCT','BKK07','LCIT','LCB05','ECT','BKK08','LCD','LCB03','TIPS','LCB02','MON','BKK03','LCT','BKK12','BKK50') AS DST_CODE,
 decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,
  NVL(to_char(ECM.VGM_GROSS_WEIGHT),'NOVGM') AS VGM,
 nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,
 nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,
 (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,
 decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','','3') AS OUT_TRANSPORT_MODE_CODE, 
 nvl((SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN), ' ')AS INLAND_CARR_CODE,
   (SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN)AS INLAND_CARR_TP_MEAN_CODE,
 decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,
 decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C
 FROM TMS_OWNER.EDI_CNTR_MOVE ECM
 WHERE ECM.CNTR_OPERATOR_CODE IN ('ZIM')
 AND ECM.MOVE_TYPE_AN = 'LD'
 AND ECM.OUT_TRANSPORT_MODE_CODE IN ('BARGE')
 AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')
 AND ECM.AREA_C LIKE :BERTH 
 AND ECM.CNTR_AN IN (:CNTR)
 )


 
