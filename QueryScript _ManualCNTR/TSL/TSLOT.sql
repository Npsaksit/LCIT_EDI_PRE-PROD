 (
 SELECT
    ECM.CNTR_AN,
    decode(ECM.CONTAINER_TYPE_CODE, '2210', '20GP', '2200', '20GP', '2250', '20GP', '22G1', '20GP', '2230', '20RF', '2251', '20OT', '2263', '20FR', '22T6', '20TK', '2500', '20HQ', '4300', '40GP', '4310', '40GP', '4351', '40OT', '4250', '40OT', '4363', '40FR', '4364', '40FR', '4351', '40OT', '4500', '40HQ', '4530', '40RH', '4532', '40RH', '45R0', '40RH', '45R1', '40RH', '4550', '45OT', '45G1', '40HQ', ECM.CONTAINER_TYPE_CODE) AS CONTAINER_TYPE_CODE,
    TO_CHAR(ECM.ACTIVITY_TM , 'DD-MM-YYYY HH24:MI') AS ACTIVITY_TM,
    (CASE
        WHEN ECM.LADEN_INDICATOR_AN = 'F'
        AND ECM.MOVE_TYPE_AN = 'GO'
        AND ECM.DST_CODE = 'LCGF1'
        OR ECM.DST_CODE = 'LCGF2'
        OR ECM.DST_CODE = 'LCGF3'
        OR ECM.DST_CODE = 'BKGF2'
        OR ECM.DST_CODE = 'LCKCT'
        OR ECM.DST_CODE = 'LCNO5'
        OR ECM.DST_CODE = 'LCPHD'
        OR ECM.DST_CODE = 'BKBCD'
        OR ECM.DST_CODE = 'BKBC2'
        OR ECM.DST_CODE = 'LCGFD' THEN 'FT'
        WHEN ECM.MOVE_TYPE_AN = 'RO' THEN 'RL'
        ELSE 'FG'
    END) STATUS,
    DECODE((DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N), '', DECODE(ECM.SHIPPING_STATUS_CODE, 'ST', ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))), 'B5', 'LCNO5', 'C3', 'LCNC3') TERMIANL,
    'THLCB' PORT,
     REPLACE(ECM.DST_CODE,',','') DESTINATION,
    'COC' OWNER_STATUS,
    ECM.MOVE_TYPE_AN,
    LADEN_INDICATOR_AN,
    'No Vessel' VESSEL_NM_AN,
    ECM.EQP_STATUS_CODE
 FROM
    TMS_OWNER.EDI_CNTR_MOVE ECM
 WHERE
    ECM.CNTR_OPERATOR_CODE IN ('TSL')
    AND ECM.MOVE_TYPE_AN IN ('GO', 'RO')
    AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RAIL', 'GATE', 'LOLO')
    AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI') 
    AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N), '', DECODE(ECM.SHIPPING_STATUS_CODE, 'ST', ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH
    AND ECM.LADEN_INDICATOR_AN LIKE 'F' 
     AND ECM.CNTR_AN IN (:CNTR))
 UNION ALL (
 SELECT
 ECM.CNTR_AN,
 decode(ECM.CONTAINER_TYPE_CODE, '2210', '20GP', '2200', '20GP', '2250', '20GP', '22G1', '20GP', '2230', '20RF', '2251', '20OT', '2263', '20FR', '22T6', '20TK', '2500', '20HQ', '4300', '40GP', '4310', '40GP', '4351', '40OT', '4250', '40OT', '4363', '40FR', '4364', '40FR', '4351', '40OT', '4500', '40HQ', '4530', '40RH', '4532', '40RH', '45R0', '40RH', '45R1', '40RH', '4550', '45OT', '45G1', '40HQ', ECM.CONTAINER_TYPE_CODE) AS CONTAINER_TYPE_CODE,
 TO_CHAR(ECM.ACTIVITY_TM , 'DD-MM-YYYY HH24:MI') AS ACTIVITY_TM,
( CASE
    WHEN ECM.LADEN_INDICATOR_AN = 'E'
        AND ECM.DST_CODE = 'LCGF1'
        OR ECM.DST_CODE = 'LCGF2'
        OR ECM.DST_CODE = 'LCGF3'
        OR ECM.DST_CODE = 'BKGF2'
        OR ECM.DST_CODE = 'LCKCT'
        OR ECM.DST_CODE = 'LCNO5'
        OR ECM.DST_CODE = 'LCPHD'
        OR ECM.DST_CODE = 'BKBCD'
        OR ECM.DST_CODE = 'BKBC2'
        OR ECM.DST_CODE = 'LCGFD' THEN 'MT'
        WHEN ECM.MOVE_TYPE_AN = 'RO' THEN 'RL'
        ELSE 'MS'
    END) STATUS,
 DECODE((DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N), '', DECODE(ECM.SHIPPING_STATUS_CODE, 'ST', ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))), 'B5', 'LCNO5', 'C3', 'LCNC3') TERMIANL,
 'THLCB' PORT,
 REPLACE(ECM.DST_CODE,',','') DESTINATION,
 'COC' OWNER_STATUS,
 ECM.MOVE_TYPE_AN,
 LADEN_INDICATOR_AN,
 'No Vessel' VESSEL_NM_AN,
 ECM.EQP_STATUS_CODE
 FROM
 TMS_OWNER.EDI_CNTR_MOVE ECM
 WHERE
 ECM.CNTR_OPERATOR_CODE IN ('TSL')
    AND ECM.MOVE_TYPE_AN IN ('GO', 'RO')
        AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RAIL', 'GATE', 'LOLO')
            AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI') 
                AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N), '', DECODE(ECM.SHIPPING_STATUS_CODE, 'ST', ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH
                AND ECM.LADEN_INDICATOR_AN LIKE 'E'
                 AND ECM.CNTR_AN IN (:CNTR) )